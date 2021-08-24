<template>
    <shader-doodle shadertoy id="buffer0">
        <sd-texture src="matcap6.png" name="matcap" />

        <component :is="'script'" type="x-shader/x-fragment">
            {{ shader }}
        </component>
    </shader-doodle>

    <shader-doodle shadertoy v-if="ready">
        <sd-texture
            shadow-root="#buffer0"
            force-update
            src="canvas"
            name="buffer0"
        />

        <component :is="'script'" type="x-shader/x-fragment">
            {{ fxaa }}
        </component>
    </shader-doodle>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue'
import utils from './shaders/utils.fs?raw'
import shader from './shaders/sdf.fs?raw'
import fxaa from './shaders/fxaa.fs?raw'

export default defineComponent({
    setup() {
        return {
            shader: utils + shader,
            fxaa,
            ready: ref(false),
        }
    },
    mounted() {
        setTimeout(() => (this.ready = true), 1000)
    },
})
</script>

<style lang="scss">
shader-doodle {
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    width: 200%;
    height: 200%;
    background: radial-gradient(white, #dedede);
    transform: scale(0.5);
    transform-origin: top left;
}
shader-doodle + shader-doodle {
    pointer-events: none;
}
#buffer0 {
    opacity: 0;
}
</style>
