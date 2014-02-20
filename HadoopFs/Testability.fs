module HadoopFs.Testability

open HadoopFs.Core
open HadoopFs.IO
open System.Collections.Generic

/// Emulates a real map-reduce session, piping all data through the mapper, performing a sort and grouping on key, and then piping it through the reducer.
let doMapReduce(reader,writer) mapper reducer = 
    let mapResults = List()
    doMapCustom (reader, mapResults.Add) mapper
    
    // order results based on key.
    let groupedMapResults = 
        mapResults
        |> Seq.map (fun res -> 
               res, 
               res
               |> Helpers.intoKeyValue
               |> fst)
        |> Seq.sortBy snd
        |> Seq.map fst
        |> Seq.toList
    
    // do the reduce outputting data to the supplied writer.
    doReduceCustom (groupedMapResults, writer) reducer

/// Emulates a real map-reduce session, piping all data provided as a list through the mapper and reducer and returning the output as a sequence of key/value tuples (split on tab).
let doInMemoryMapReduce mapper reducer input = 
    let reduceResults = List()
    doMapReduce(input,reduceResults.Add) mapper reducer
    reduceResults
    |> Seq.map Helpers.intoKeyValue
    |> Seq.cache