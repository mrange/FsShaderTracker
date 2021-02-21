module FsTracker =
  open System
  open System.Diagnostics
  open System.Numerics
  open System.Drawing
  open System.Runtime.InteropServices
  open System.Text
  open System.Windows.Forms

  module Common =
    type OnExit(f: unit -> unit) =
      class
        interface IDisposable with
          member x.Dispose () =
            f ()
      end
    let onExit f = new OnExit(f)
    let onExitFreeCoTaskMem p = onExit <| (fun () -> Marshal.FreeCoTaskMem p)

  module Units =
    type [<Measure>] sec
    type [<Measure>] hz
    type [<Measure>] bpm

  open Units

  [<RequireQualifiedAccess>]
  type ShaderInputValue = 
    | V1        of float32
    | V2        of Vector2
    | V3        of Vector3
    | V4        of Vector4
    | Sampler2D of Unit

  type ShaderInput = {
    Key   : string
    Value : ShaderInputValue
  }

  type ShaderEffect = {
    Inputs : ShaderInput array
  }

  let now = 
    let sw = Stopwatch ()
    sw.Start ()
    fun () -> (float sw.ElapsedMilliseconds/1000.)*1.0<sec>

  module WindowsForms =
    open Common

    open type FsTracker.OpenGL.Native

    let vertexShader = """#version 460

#define SCREEN_LOADER

layout (location=0) in vec2 inVer;
out vec2 p;
out vec2 q;

out gl_PerVertex
{
  vec4 gl_Position;
};

void main()
{
  gl_Position=vec4(inVer,0.0,1.0);
  p=inVer;
  q=0.5*inVer+0.5;
}
"""

    let fragmentShader = """#version 460

#define SCREEN_LOADER

precision mediump float;

in vec2 p;
in vec2 q;

layout (location=0) out vec4 fragColor;

layout (location=0) uniform float time;
layout (location=1) uniform vec2 resolution;

void main(void)
{
  fragColor = vec4(1.0, 0.5, 0.25, 1.0);
}
"""

    let pixelFormatDescriptor =
      let mutable pfd = PIXELFORMATDESCRIPTOR.Create ()
      pfd.dwFlags               <- PFD_DRAW_TO_WINDOW|||PFD_SUPPORT_OPENGL|||PFD_DOUBLEBUFFER
      pfd.iPixelType            <- PFD_TYPE_RGBA
      pfd.cColorBits            <- 24uy
      pfd.cRedBits              <- 0uy
      pfd.cRedShift             <- 0uy
      pfd.cGreenBits            <- 0uy
      pfd.cGreenShift           <- 0uy
      pfd.cBlueBits             <- 0uy
      pfd.cBlueShift            <- 0uy
      pfd.cAlphaBits            <- 8uy
      pfd.cAlphaShift           <- 0uy
      pfd.cAccumBits            <- 0uy
      pfd.cAccumRedBits         <- 0uy
      pfd.cAccumGreenBits       <- 0uy
      pfd.cAccumBlueBits        <- 0uy
      pfd.cAccumAlphaBits       <- 0uy
      pfd.cDepthBits            <- 32uy
      pfd.cStencilBits          <- 0uy
      pfd.cAuxBuffers           <- 0uy
      pfd.iLayerType            <- PFD_MAIN_PLANE
      pfd.bReserved             <- 0uy
      pfd.dwLayerMask           <- 0u
      pfd.dwVisibleMask         <- 0u
      pfd.dwDamageMask          <- 0u
      pfd

    let check nm res =
      if res  = Unchecked.defaultof<_> then
        let e = Marshal.GetLastWin32Error ()
        failwithf "%s failed with 0x%x" nm e
      else
        res

    let check_link_status (opengl : OpenGlFunctions) nm id =
      let mutable res = -1
      opengl.glGetProgramiv.Invoke (id, GL_LINK_STATUS, &res)
      if res = 0 then
        let buf = Array.zeroCreate 1024
        let mutable bufLen = 0
        opengl.glGetProgramInfoLog.Invoke (id, 1024, &bufLen, buf)
        let err = Encoding.ASCII.GetString (buf, 0, bufLen)
        failwithf "Link status failed for '%s' with: %s" nm err
      else
        id

    let run () =
      Application.SetHighDpiMode HighDpiMode.SystemAware |> ignore
      Application.EnableVisualStyles ()
      Application.SetCompatibleTextRenderingDefault false

      use form = new Form ()
      form.ClientSize <- Size (1920, 1200)
      form.Text       <- "Hello"

      let hwnd        = form.Handle
      let hdc         = GetDC hwnd

      let mutable pfd = pixelFormatDescriptor
      let pf = 
        ChoosePixelFormat (hdc, &pfd) 
        |> check "ChoosePixelFormat"

      SetPixelFormat (hdc, pf, &pfd) 
      |> check "SetPixelFormat" 
      |> ignore

      let hglrc = 
        wglCreateContext hdc 
        |> check "wglCreateContext"

      wglMakeCurrent (hdc, hglrc) 
      |> check "wglMakeCurrent" 
      |> ignore

      let opengl = new OpenGlFunctions ()
      let check_link_status = check_link_status opengl

      let vsh = Marshal.StringToCoTaskMemAnsi vertexShader
      use freeVsh = onExitFreeCoTaskMem vsh

      let fsh = Marshal.StringToCoTaskMemAnsi fragmentShader
      use freeFsh = onExitFreeCoTaskMem fsh

      let vsid = 
        opengl.glCreateShaderProgramv.Invoke (GL_VERTEX_SHADER, 1, [|vsh|]) 
        |> check_link_status "glCreateShaderProgramv"

      let fsid = 
        opengl.glCreateShaderProgramv.Invoke (GL_FRAGMENT_SHADER, 1, [|fsh|]) 
        |> check_link_status "glCreateShaderProgramv"

      let mutable pid = 0u
      opengl.glGenProgramPipelines.Invoke (1, &pid)
      opengl.glBindProgramPipeline.Invoke pid
      opengl.glUseProgramStages.Invoke (pid, GL_VERTEX_SHADER_BIT, vsid)
      opengl.glUseProgramStages.Invoke (pid, GL_FRAGMENT_SHADER_BIT, fsid)

      let onPaint o e =
        let size = form.ClientSize
        glViewport (0, 0, size.Width, size.Height)
        opengl.glBindFramebuffer.Invoke (GL_DRAW_FRAMEBUFFER, 0u)
        opengl.glProgramUniform1f.Invoke (fsid, 0 , 0.F)
        opengl.glProgramUniform1f.Invoke (fsid, 0 , 0.F)
        opengl.glProgramUniform2f.Invoke (fsid, 0 , float32 size.Width, float32 size.Height)
        glRects (-1s, -1s, 1s, 1s)
        SwapBuffers hdc |> ignore

        ()
      form.Paint.AddHandler (PaintEventHandler onPaint)

      Application.Run form


module FsMyDemo =
  open FsTracker
  open Units

  let tempo = 128.0<bpm>

  let intro =
    [|
      
    |]

open System  

[<EntryPoint>]
[<STAThread>]
let main argv =
  FsTracker.WindowsForms.run ()
  0
