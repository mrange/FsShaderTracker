module FsMyDemo =
  let fragmentShader = """#version 460
#define SCREEN_LOADER

precision mediump float;

in vec2 p;
in vec2 q;

layout (location=0) out vec4 fragColor;

layout (location=0) uniform float time;
layout (location=1) uniform vec2 resolution;

#define PSIN(x) (0.5+0.5*sin(x))

void main(void) {
  fragColor = vec4(PSIN(time), q.x, q.y, 1.0);
}
"""

  open FsTracker
  open Units

  let tempo = 128.0<bpm>

  let myModule : Module = 
    {
      FragmentShader = fragmentShader
    }

  let run () = WindowsForms.run myModule

open System  

[<EntryPoint>]
[<STAThread>]
let main argv =
  FsMyDemo.run ()
  0
