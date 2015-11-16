// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

// http://www.codeproject.com/Articles/773451/Fsharp-Asynchronous-Workflows

open Newtonsoft.Json
open raw_cp.Config
open raw_cp.CommandLine
open System
open System.IO
open System.Threading.Tasks

type FileGrp = {Date: DateTime; Files: seq<FileInfo>}

let load filename =
     JsonConvert.DeserializeObject<raw_cp.Config.Config>(File.ReadAllText(filename))

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
        let di = Directory.CreateDirectory(dest)
        async {
            let d = Path.Combine(di.FullName, Path.GetFileName(f))
            use reader = File.Open(f, FileMode.Open)
            use writer = File.Create(d)
            
            printfn "Copying: %s -> %s" f d
            
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
    
[<EntryPoint>]
let main argv =
    let opts = argv |> raw_cp.CommandLine.parseCommandLine

    let config = load "./raw-cp-config.json"
    let files = fileGroups config.SourceFolder
    let first = (files |> Seq.head)
    let filenames = first.Files |> Seq.map (fun fi -> fi.FullName)
    let destFolder = destinationFolder config.DestinationFolder first.Date opts.Description config.Cameras.[opts.Camera]
    let results = copyFiles filenames destFolder
    results.Wait()

    0 // return an integer exit code
