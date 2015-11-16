namespace raw_cp.Config

open System.Collections.Generic

type Config() =
    member val SourceFolder = "" with get, set
    member val DestinationFolder = "" with get, set
    member val Cameras = new Dictionary<string,string>() with get, set