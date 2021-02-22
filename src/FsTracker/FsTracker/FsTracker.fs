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

type ShaderInputValue =
  | V1    of float32
  | V2    of Vector2
  | V3    of Vector3
  | V4    of Vector4

type Environment = 
  {
    Time        : float32
    Resolution  : Vector2
  }

type ShaderInput =
  | Const       of ShaderInputValue
  | Env         of (Environment -> ShaderInputValue)
  | Unary       of ShaderInput*(ShaderInputValue -> ShaderInputValue)
  | Binary      of ShaderInput*ShaderInput*(ShaderInputValue -> ShaderInputValue -> ShaderInputValue)

module ShaderInputs =
  let time = Env <| fun e -> 
    V1 e.Time

  let resolution = Env <| fun e -> 
    V1 e.Time

type VertexShader = {
  Name    : string
  Source  : string
}

type FragmentShader = {
  Name    : string
  Source  : string
}

type Module = 
  {
    FragmentShader  : string
  }

type Step =
  {
    Effects   : (Environment -> int*string) array
    Inputs    : (Environment -> string*ShaderInput) array
  }

type Track = 
  {
    Name  : string
    Steps : Step array
  }

type Module_ = 
  {
    Name      : string
    Vertex    : VertexShader
    Mixer     : int*FragmentShader
    Effects   : FragmentShader array
    Sequence  : Track array
  }

