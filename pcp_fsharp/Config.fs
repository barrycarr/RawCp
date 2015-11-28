namespace RawCp.Config

open System.Collections.Generic

type Config() =
    member val AlwaysMove = false with get, set
    member val SourceFolder = "" with get, set
    member val DestinationFolder = "" with get, set
    member val Cameras = new Dictionary<string,string>() with get, set
    member val DefaultCamera = "" with get, set

module Helpers =
    open System.Reflection
    open System.IO
    open System
    open Newtonsoft.Json
    open RawCp.Either

    let load fn =
        let locations = [
            Directory.GetCurrentDirectory();
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) ]

        let rec loadRec locs fn =
            match locs with
            | [] -> None
            | hd::xs -> 
                let path = Path.Combine( (locs |> List.head), fn)
                if File.Exists(path) 
                    then Some(path)
                else loadRec (locs |> List.tail) fn

        match loadRec locations fn with
        | Some(fn) -> success (JsonConvert.DeserializeObject<Config>(File.ReadAllText(fn)))
        | None -> failure "Config file not found! Searched the current directory; RawCp's directory and your home directory!"