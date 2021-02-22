namespace FsTracker

open System
open System.Diagnostics
open System.Numerics
open System.Runtime.InteropServices

module Units =
  type [<Measure>] sec
  type [<Measure>] hz
  type [<Measure>] bpm
open Units

module Common =
  type OnExit(f: unit -> unit) =
    class
      interface IDisposable with
        member x.Dispose () =
          f ()
    end
  let onExit f = new OnExit(f)
  let onExitFreeCoTaskMem p = onExit <| (fun () -> Marshal.FreeCoTaskMem p)

  let private sw = 
    let sw = Stopwatch ()
    sw.Start ()
    sw

  let now () = (float sw.ElapsedMilliseconds/1000.)*1.0<sec>

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

type Module = 
  {
    FragmentShader : string
  }

