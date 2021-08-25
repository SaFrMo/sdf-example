// https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
/** random, given seed*/
float rand(float n) {
    return fract(sin(n) * 43758.5453123);
}

// polynomial smooth min (k = 0.1);
// https://iquilezles.org/www/articles/smin/smin.htm
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * (1.0 / 4.0);
}

/**
 * Return the normalized direction to march in from the eye point for a single pixel.
*/
vec3 rayDirection(vec2 fragCoord, float fov, vec2 size) {
    vec2 xy = fragCoord - size * 0.5;
    float z = size.y / tan(radians(fov) * 0.5);
    return normalize(vec3(xy, z));
}
/** Normalized raymarch direction using the default 45deg FOV and shader resolution */
vec3 rayDirection(vec2 fragCoord) {
    return rayDirection(fragCoord, 45., iResolution.xy);
}

// SDF SHAPES
/** add a sphere with specified radius at specified point */
float sdSphere(vec3 point, vec3 center, float radius) {
    return length(point - center) - radius;
}

/** unit sphere at origin */
float sdSphere(vec3 point) {
    return sdSphere(point, vec3(0.), 1.);
}

/** get matcap UV coordinates from camera and normal */
// https://www.clicktorelease.com/blog/creating-spherical-environment-mapping-shader/
vec2 matcapUv(vec3 camera, vec3 normal) {
    vec3 reflected = reflect(normalize(camera), normalize(normal));
    float m = 2. * sqrt(pow(reflected.x, 2.) +
        pow(reflected.y, 2.) +
        pow(reflected.z + 1., 2.));
    return reflected.xy / m + .5;
}