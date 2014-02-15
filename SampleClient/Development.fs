// Examples of how to use HadoopFs during development of MR jobs
module Development

open HadoopFs
open HadoopFs.Core
open HadoopFs.Samples
open HadoopFs.Testability

let ``Testing your map or reduce functions directly``() = 
    // Just call your code directly - no frameworks needed :-)
    let mapOutput = WordCount.Mapper "hello hello there"
    let reduceOutput = WordCount.Reducer("hello", [ "1"; "2" ])
    ignore()

let ``Testing calls to map with datasets``() = 
    // Run the WordCount mapper over an input set from an in-memory collection, and output the results to the interactive shell
    // Alternatively you can use one of the in-built input / output streams e.g. file system, console etc.
    let data = [ "hello hello there"; "hello goodbye"; "there you are" ]
    doMapCustom (MultiValue WordCount.Mapper, IO.Readers.Collection(data), printfn "%s")
    ignore()

let ``Testing calls to reduce with datasets``() = 
    // Note that doReduce always expects keys to have been sorted (this is done automatically by Hadoop)
    let data = [ "hello\t2"; "hello\t1"; "goodbye\t1" ]
    doReduceCustom (SingleValue WordCount.Reducer, IO.Readers.Collection(data), printfn "%s")
    ignore()

let ``Testing combined map and reduce``() = 
    // Do a "full" map reduce using any input source piped through and map and reduce.
    let data = [ "hello hello there"; "hello goodbye"; "there you are" ]
    let reducedOutput = doInMemoryMapReduce (MultiValue WordCount.Mapper, SingleValue WordCount.Reducer, data)
    reducedOutput |> Seq.iter (printfn "%A")