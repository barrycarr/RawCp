﻿open RawCp.Config
open RawCp.CommandLine
open RawCp.Either
open System
open System.IO
open System.Threading.Tasks

type FileGrp = {Date: DateTime; Files: seq<FileInfo>}

type Params = {
    Config: Config; 
    Opts: CommandLineOptions;
    Filenames: seq<String>;
}

let printLegend opts (conf: Config)  dest =
    let mthd = if opts.Move
                  then "MOVING" 
                  else "COPYING"
    printfn "RawCp: v0.1"
    printfn "  %s files" mthd
    printfn "  from: %s" conf.SourceFolder
    printfn "  to:   %s" dest;


let fileGroups dir =
    let dirInfo = DirectoryInfo(dir)
    dirInfo.GetFiles("*.*") 
        |> Array.sortBy (fun fi -> fi.LastWriteTime.Date)
        |> Seq.groupBy (fun fi -> fi.LastWriteTime.Date)
        |> Seq.map(fun (dt, infos) -> {Date = dt.Date; Files = infos})


let copyFiles files dest =
    let asyncAwaitVoidTask (t: Task) =
        Async.AwaitIAsyncResult(t) |> Async.Ignore

    let copyFile dest f =
        // http://www.codeproject.com/Articles/773451/Fsharp-Asynchronous-Workflows
        let di = Directory.CreateDirectory(dest)
        async {
            let d = Path.Combine(di.FullName, Path.GetFileName(f))
            use reader = File.Open(f, FileMode.Open)
            use writer = File.Create(d)
            
            printfn "  %s -> %s" f d
            
            do! reader.CopyToAsync(writer) |> asyncAwaitVoidTask
            do! writer.FlushAsync() |> asyncAwaitVoidTask
        } |> Async.StartAsTask
            
    Task.WhenAll(files |> Seq.map (copyFile dest)) 

    
let destinationFolder destFolder (date: DateTime) desc camera =
    let subject = sprintf "%s %s" (date.ToString("yyyyMMdd")) desc 
    Path.Combine(
        destFolder, 
        date.ToString("yyyy"), 
        date.ToString("MM MMMM"), 
        subject.TrimEnd(), 
        camera)


let deleteSourceFiles move files =
    if move then
        for f in files do
            File.Delete(f)

let findFiles (config: Result<Config>) =
    let getFilenames files =
        let first = (files |> Seq.head)
        first.Files |> Seq.map (fun fi -> fi.FullName)

    match config with
    | Failure s -> failure s
    | Success c -> 
        let files = fileGroups c.SourceFolder
        if  files |> Seq.isEmpty then
            failure (sprintf "No files found in %s" c.SourceFolder)
        else
            success (getFilenames files)
           
let getConfig configFilename (parameters: Result<Params>) =
    match parameters with
    | Failure x -> failure x
    | Success p -> 
        let config = Helpers.load configFilename
        match config with
        | Failure c -> failure c
        | Success c ->
            success {p with Config = c} 

let getOptions args (parameters: Result<Params>) =
    match parameters with
    | Failure x -> Failure x
    | Success p ->
        let                         
[<EntryPoint>]
let main argv =
    let emptyParams = success {
        Config = Config();
        Opts = RawCp.CommandLine.emptyCommandLineOptions;
        Filenames = Seq.empty<String>
    }

    let params = getConfig "rawcp-config.json" emptyParams
        |> getOptions argv

    let config = Helpers.load "rawcp-config.json"
    let opts = RawCp.CommandLine.parseCommandLine argv config
    let filenames = findFiles config
    let destFolder = destinationFolder config.DestinationFolder first.Date opts.Description config.Cameras.[opts.Camera]

    printLegend opts config destFolder

    let results = copyFiles filenames destFolder
    results.Wait()

    deleteSourceFiles opts.Move filenames
    0 // return an integer exit code
