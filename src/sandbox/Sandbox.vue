<template>
    <shader-doodle shadertoy id="buffer0">
        <sd-texture src="matcap6.png" name="matcap" />

        <component :is="'script'" type="x-shader/x-fragment">
            {{ shader }}
        </component>
    </shader-doodle>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import utils from './utilsv2.fs?raw'
import shader from './sandbox.fs?raw'

export default defineComponent({
    setup() {
        return {
            shader: utils + shader,
        }
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
</style>
