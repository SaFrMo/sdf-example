const float EPSILON = 0.0001;
const vec3 EPSILON_V3 = vec3(EPSILON, 0., 0.);
const float MAX_DISTANCE = 1000.;
const int MAX_STEPS = 32;

const vec3 BACKGROUND_COLOR = vec3(0.);

/** add a sphere with specified radius at specified point */
float sphere(vec3 point, vec3 center, float radius) {
    return length(point - center) - radius;
}

/** unit sphere at origin */
float sphere(vec3 point) {
    return sphere(point, vec3(0.), 1.);
}

/** scene function*/
float scene(vec3 point) {
    float sphere0 = sphere(point);

    return sphere0;
}
/**
 * Return the normalized direction to march in from the eye point for a single pixel.
*/
vec3 rayDirection(float fov, vec2 size, vec2 fragCoord) {
    vec2 xy = fragCoord - size * 0.5;
    float z = size.y / tan(radians(fov) * 0.5);
    return normalize(vec3(xy, z));
}
/** Normalized raymarch direction using the default 45deg FOV and shader resolution */
vec3 rayDirection(vec2 fragCoord) {
    return rayDirection(45., iResolution.xy, fragCoord);
}

vec3 calculateNormal(vec3 point) {
    float x = scene(point + EPSILON_V3.xyy) - scene(point - EPSILON_V3.xyy);
    float y = scene(point + EPSILON_V3.yxy) - scene(point - EPSILON_V3.yxy);
    float z = scene(point + EPSILON_V3.yyx) - scene(point - EPSILON_V3.yyx);

    return normalize(vec3(x, y, z));
}

/** raymarch to determine object position and color */
vec3 raymarch(vec3 origin, vec3 dir) {
    float distanceTraveled = 0.;
    for(int i = 0; i < MAX_STEPS; ++i) {
        vec3 pos = origin + dir * distanceTraveled;
        float distanceToClosestEntity = scene(pos);

        if(distanceToClosestEntity <= EPSILON) {
            // get normal
            vec3 normal = calculateNormal(pos);

            // place lights
            vec3 lightPos = vec3(2., -5., 3.);

            // light object
            vec3 dirToLights = normalize(pos - lightPos);
            float lightIntensity = max(0., dot(normal, dirToLights));

            // object diffuse color
            vec3 col = vec3(0., 0., 1.);

            // return the color for this entity
            return col * lightIntensity;
        }

        distanceTraveled += distanceToClosestEntity;
        if(distanceTraveled > MAX_DISTANCE) {
            break;
        }
    }
    return BACKGROUND_COLOR;
}

void main() {
    // position camera
    vec3 cameraPos = vec3(0., 0., -10.);
    // calculate raymarch direction
    vec3 dir = rayDirection(gl_FragCoord.xy);

    // execute raymarch and get color
    vec3 col = raymarch(cameraPos, dir);

    gl_FragColor = vec4(col, 1.);
}
