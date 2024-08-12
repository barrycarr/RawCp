namespace RawCp.Config

open System.IO
open FSharp.Data


type Config = JsonProvider<"J:\\OneDrive\\Dev\\RawCp\\_RawCp\\rawcp-config.json">

module Test =
    let simpleTest =
        let config = Config.Parse(File.ReadAllText "J:\\OneDrive\\Dev\\RawCp\\_RawCp\\rawcp-config.json")
        printfn $"Config Source Folder: %s{config.SourceFolder}"
        printfn $"Config Destination Folder: %s{config.DestinationFolder}"
        printfn $"Config Default Camera: %s{config.DefaultCamera}"
    
module Helpers =
    open System.Reflection
    open System.IO
    open System
    
    let load fileName =
        let locations = [
            "J:\\OneDrive\\Dev\\RawCp\\_RawCp\\";
            Directory.GetCurrentDirectory();
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) ]

        let rec loadRec locs fName =
            match locs with
            | [] -> None
            | hd::xs -> 
                let path = Path.Combine( hd, fName)
                if File.Exists(path) 
                    then Some(path)
                else loadRec xs fName

        match loadRec locations fileName with
        | Some(f) -> Config.Parse(File.ReadAllText f)
        | None -> failwith "Config file not found! Searched the current directory; RawCp's directory and your home directory!"
        
    let findCamera (config: Config.Root) cameraId =
        config.Cameras |> Seq.tryFind (fun (camera) -> camera.Id = cameraId) |> Option.map (_.Value)