// based on tutorials from Jamie Wong and Michael Walczyk:
// http://jamie-wong.com/2016/07/15/ray-marching-signed-distance-functions/
// https://michaelwalczyk.com/blog-ray-marching.html
// as well as resources from Inigo Quilez:
// https://iquilezles.org/www/articles/distfunctions/distfunctions.htm

precision highp float;
const float EPSILON = 0.00001;
const vec3 EPSILON_V3 = vec3(EPSILON, 0., 0.);
const float MAX_DISTANCE = 1000.;
const int MAX_STEPS = 32;

vec4 backgroundColor() {
    vec2 st = gl_FragCoord.xy / iResolution.xy;
    float lightness = 1. - distance(st, vec2(0.5));
    return vec4(vec3(lightness), 1.) + 0.4;
}

uniform sampler2D matcap;

/** scene function*/
float scene(vec3 point) {
    float sphere0 = sdSphere(point);
    vec3 t = point + iTime * 0.7;
    float displacement = sin(5. * t.x) * sin(3. * t.y) * sin(3. * t.z) * 0.1;
    sphere0 += displacement;

    float result = sphere0;

    // second sphere
    float x = (iCurrentMouse.x / iResolution.x) * 5. - 2.5;
    float y = (iCurrentMouse.y / iResolution.y) * 5. - 2.5;
    float sphere1 = sdSphere(point, vec3(x, y, 0.), 0.5);
    // distort second sphere
    displacement = sin(-5. * t.x) * sin(-5. * t.y) * sin(-5. * t.z) * 0.1;
    sphere1 += displacement;

    // combine
    result = smin(sphere0, sphere1, 0.4);

    return result;
}

vec3 calculateNormal(vec3 point) {
    float x = scene(point + EPSILON_V3.xyy) - scene(point - EPSILON_V3.xyy);
    float y = scene(point + EPSILON_V3.yxy) - scene(point - EPSILON_V3.yxy);
    float z = scene(point + EPSILON_V3.yyx) - scene(point - EPSILON_V3.yyx);

    return normalize(vec3(x, y, z));
}

/** raymarch to determine object position and color */
vec4 raymarch(vec3 origin, vec3 dir) {
    float distanceTraveled = 0.;
    for(int i = 0; i < MAX_STEPS; ++i) {
        vec3 pos = origin + dir * distanceTraveled;
        float distanceToClosestEntity = scene(pos);

        if(distanceToClosestEntity < EPSILON) {
            // get normal
            vec3 normal = calculateNormal(pos);

            // MATCAP
            vec2 mUv = matcapUv(origin, normal);
            return texture2D(matcap, mUv);

            /* 
            // BASIC DIFFUSE LIGHTING
            // place lights
            vec3 lightPos = vec3(2., -5., 3.);

            // light object
            vec3 dirToLights = normalize(pos - lightPos);
            float lightIntensity = max(0., dot(normal, dirToLights));

            // object diffuse color
            vec3 col = vec3(0., 0., 1.);

            // return the color for this entity
            return vec4(col * lightIntensity, 1.);
            */
        }

        distanceTraveled += distanceToClosestEntity;
        if(distanceTraveled > MAX_DISTANCE) {
            break;
        }
    }
    return backgroundColor();
}

void main() {
    // position camera
    vec3 cameraPos = vec3(0., 0., -10.);
    // calculate raymarch direction
    vec3 dir = rayDirection(gl_FragCoord.xy);

    // execute raymarch and get color
    vec4 col = raymarch(cameraPos, dir);

    gl_FragColor = col;
}
