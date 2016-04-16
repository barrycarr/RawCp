module RawCp.FileHandling

open System
open System.IO
open System.Threading.Tasks

open RawCp.Config
open RawCp.Either
open RawCp.Params
open RawCp.CommandLineOptions

type FileGrp = {Date: DateTime; Files: seq<FileInfo>}

let fileGroups dir =
    let dirInfo = DirectoryInfo(dir)
    dirInfo.GetFiles("*.*") 
        |> Array.sortBy (fun fi -> fi.LastWriteTime.Date)
        |> Seq.groupBy (fun fi -> fi.LastWriteTime.Date)
        |> Seq.map(fun (dt, infos) -> {Date = dt.Date; Files = infos})


let findFiles (config: Result<Config>) =
    let getFilenames files =
        let first = (files |> Seq.head)
        let files = first.Files |> Seq.map (fun fi -> fi.FullName)
        (first.Date, files)

    match config with
    | Failure s -> failure s
    | Success c -> 
        let files = fileGroups c.SourceFolder
        if  files |> Seq.isEmpty then
            failure (sprintf "No files found in %s" c.SourceFolder)
        else
            success (getFilenames files)

let getFilenames p =
    let filenames = findFiles (success p.Config)
    match filenames with
    | Failure x -> failure x
    | Success (d, f) -> success {p with Date = d; Filenames = f}

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


let deleteSourceFiles move files =
    if move then
        for f in files do
            File.Delete(f)


let destinationFolder (p: Params) =
    let subject = sprintf "%s %s" (p.Date.ToString("yyyyMMdd")) p.Opts.Description
    Path.Combine(
        p.Config.DestinationFolder,
        p.Date.ToString("yyyy"),
        p.Date.ToString("MM MMMM"),
        subject.TrimEnd(),
        p.Config.Cameras.[p.Opts.Camera])

