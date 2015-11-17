open RawCp.Config
open RawCp.CommandLine
open System
open System.IO
open System.Threading.Tasks

type FileGrp = {Date: DateTime; Files: seq<FileInfo>}

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
        subject, 
        camera)


let deleteSourceFiles move files =
    if move then
        for f in files do
            File.Delete(f)

    
[<EntryPoint>]
let main argv =
    let config = Helpers.load "rawcp-config.json"
    let opts = RawCp.CommandLine.parseCommandLine argv config
    let files = fileGroups config.SourceFolder
    let first = (files |> Seq.head)
    let filenames = first.Files |> Seq.map (fun fi -> fi.FullName)
    let destFolder = destinationFolder config.DestinationFolder first.Date opts.Description config.Cameras.[opts.Camera]

    printLegend opts config destFolder

    let results = copyFiles filenames destFolder
    results.Wait()

    deleteSourceFiles opts.Move filenames
    0 // return an integer exit code
