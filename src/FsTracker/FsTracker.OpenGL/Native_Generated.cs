namespace FsTracker.OpenGL
{
  using System;
  using System.Runtime.InteropServices;

  public static partial class Native
  {
    public class OpenGlFunctions
    {
      public readonly Delegate_glCreateShaderProgramv     glCreateShaderProgramv     = LoadOpenGlFunction<Delegate_glCreateShaderProgramv>(@"glCreateShaderProgramv");
      public readonly Delegate_glGenProgramPipelines      glGenProgramPipelines      = LoadOpenGlFunction<Delegate_glGenProgramPipelines>(@"glGenProgramPipelines");
      public readonly Delegate_glBindProgramPipeline      glBindProgramPipeline      = LoadOpenGlFunction<Delegate_glBindProgramPipeline>(@"glBindProgramPipeline");
      public readonly Delegate_glUseProgramStages         glUseProgramStages         = LoadOpenGlFunction<Delegate_glUseProgramStages>(@"glUseProgramStages");
      public readonly Delegate_glGetProgramiv             glGetProgramiv             = LoadOpenGlFunction<Delegate_glGetProgramiv>(@"glGetProgramiv");
      public readonly Delegate_glGetProgramInfoLog        glGetProgramInfoLog        = LoadOpenGlFunction<Delegate_glGetProgramInfoLog>(@"glGetProgramInfoLog");
      public readonly Delegate_glProgramUniform1f         glProgramUniform1f         = LoadOpenGlFunction<Delegate_glProgramUniform1f>(@"glProgramUniform1f");
      public readonly Delegate_glProgramUniform2f         glProgramUniform2f         = LoadOpenGlFunction<Delegate_glProgramUniform2f>(@"glProgramUniform2f");
      public readonly Delegate_glActiveTexture            glActiveTexture            = LoadOpenGlFunction<Delegate_glActiveTexture>(@"glActiveTexture");
      public readonly Delegate_glBindSampler              glBindSampler              = LoadOpenGlFunction<Delegate_glBindSampler>(@"glBindSampler");
      public readonly Delegate_glProgramUniform1i         glProgramUniform1i         = LoadOpenGlFunction<Delegate_glProgramUniform1i>(@"glProgramUniform1i");
      public readonly Delegate_glGenerateMipmap           glGenerateMipmap           = LoadOpenGlFunction<Delegate_glGenerateMipmap>(@"glGenerateMipmap");
      public readonly Delegate_glGenFramebuffers          glGenFramebuffers          = LoadOpenGlFunction<Delegate_glGenFramebuffers>(@"glGenFramebuffers");
      public readonly Delegate_glBindFramebuffer          glBindFramebuffer          = LoadOpenGlFunction<Delegate_glBindFramebuffer>(@"glBindFramebuffer");
      public readonly Delegate_glFramebufferRenderbuffer  glFramebufferRenderbuffer  = LoadOpenGlFunction<Delegate_glFramebufferRenderbuffer>(@"glFramebufferRenderbuffer");
      public readonly Delegate_glGenRenderbuffers         glGenRenderbuffers         = LoadOpenGlFunction<Delegate_glGenRenderbuffers>(@"glGenRenderbuffers");
      public readonly Delegate_glBindRenderbuffer         glBindRenderbuffer         = LoadOpenGlFunction<Delegate_glBindRenderbuffer>(@"glBindRenderbuffer");
      public readonly Delegate_glRenderbufferStorage      glRenderbufferStorage      = LoadOpenGlFunction<Delegate_glRenderbufferStorage>(@"glRenderbufferStorage");
      public readonly Delegate_glBlitFramebuffer          glBlitFramebuffer          = LoadOpenGlFunction<Delegate_glBlitFramebuffer>(@"glBlitFramebuffer");
    }
  }
}
