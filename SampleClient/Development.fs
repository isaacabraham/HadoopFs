// Examples of how to use HadoopFs during development of MR jobs
module Development

open HadoopFs
open HadoopFs.Core
open HadoopFs.Samples
open HadoopFs.Testability

// Just call your code directly - no frameworks needed :-)
let ``Testing your map or reduce functions directly``() = 
    let mapOutput = WordCount.Mapper "hello hello there"
    let reduceOutput = WordCount.Reducer("hello", [ "1"; "2" ])
    ignore()

// Run the WordCount mapper over an input set from an in-memory collection, and output the results to the interactive shell
// Alternatively you can use one of the in-built input / output streams e.g. file system, console etc.
let ``Testing calls to map with datasets``() = 
    let data = [ "hello hello there"; "hello goodbye"; "there you are" ]
    doMapCustom (ManyOutputs WordCount.Mapper) (printfn "%s") data

// Note that doReduce always expects keys to have been sorted (this is done automatically by Hadoop)
let ``Testing calls to reduce with datasets``() = 
    let data = [ "hello\t2"; "hello\t1"; "goodbye\t1" ]
    doReduceCustom (SingleOutput WordCount.Reducer) (printfn "%s") data

// Do a "full" map reduce using an in memory data set, piped through a map and reduce.
let ``Testing combined map and reduce``() = 
    (ManyOutputs WordCount.Mapper, SingleOutput WordCount.Reducer)
    ||> doInMemoryMapReduce
    <| [ "hello hello there"; "hello goodbye"; "there you are" ]
    |> Seq.iter (printfn "%A")
