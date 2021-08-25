float scene(vec3 p) {
    float result = 1.;

    float sphere0 = sdfSphere(p, vec3(0., (abs(sin(iTime)) + 0.5) * 2., 0.), 0.5);
    float plane = sdfPlane(p);
    result = opUnion(sphere0, plane);

    return result;
}

vec4 castRay(vec3 origin, vec3 dir) {
    float distanceTraveled = 0.;
    for(int i = 0; i < MAX_STEPS; i++) {
        vec3 pos = origin + dir * distanceTraveled;

        float distanceToClosestEntity = scene(pos);
        distanceTraveled += distanceToClosestEntity;

        if(distanceToClosestEntity < EPSILON) {
            float intensity = distanceTraveled;
            // return vec4(vec3(intensity), 1.);
            break;
        }

        // prep to continue march
        if(distanceTraveled >= MAX_DISTANCE) {
            break;
        }
    }

    // TODO: write your own background color
    float intensity = 1. - distanceTraveled * 0.025;
    return vec4(vec3(intensity), 1.);
}

void main() {
    // position camera
    vec3 cameraPos = vec3(0., 1., -10.);
    // calculate raymarch direction
    vec3 dir = rayDirection(gl_FragCoord.xy);

    // execute raymarch and get color
    vec4 col = castRay(cameraPos, dir);

    gl_FragColor = col;
}
