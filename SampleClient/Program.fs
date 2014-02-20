open HadoopFs.Core
open HadoopFs.IO
open HadoopFs.Samples
open HadoopFs.Testability

// A reducer exe
//let mainReduce argv = 
//    SingleOutput WordCount.Reducer |> doReduce
//    0

[<EntryPoint>]
let mainMap argv = 
    // Push the wordcount map / reduce through the console.
    (ManyOutputs WordCount.Mapper, SingleOutput WordCount.Reducer) 
    ||> doMapReduce (Readers.TestableConsole, Writers.Console)
    0
