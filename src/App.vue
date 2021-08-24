<template>
    <shader-doodle shadertoy>
        <component :is="'script'" type="x-shader/x-fragment">
            {{ shader }}
        </component>
    </shader-doodle>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import utils from './shaders/utils.fs?raw'
import shader from './shaders/sdf-walczyk.fs?raw'

export default defineComponent({
    setup() {
        return {
            shader: utils + shader,
        }
    },
})
</script>

<style >
shader-doodle {
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    width: 100%;
    height: 100%;
    background: radial-gradient(white, #dedede);
}
</style>
