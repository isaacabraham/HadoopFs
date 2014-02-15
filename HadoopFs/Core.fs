module HadoopFs.Core

open HadoopFs.IO
open System
open System.IO

module internal Helpers = 
    /// Generates a stream of key/values grouped by adjacent key from a sequence of ordered key/value pairs.
    let groupByAdjacent (data : seq<_>) = 
        let lastKey = ref String.Empty
        let enumerator = data.GetEnumerator()
        seq { 
            let moreData = ref (enumerator.MoveNext())
            while !moreData do
                let key, value = enumerator.Current
                if !lastKey <> key then 
                    lastKey := key
                    yield key, 
                          seq { 
                              while !moreData && (!lastKey = (fst enumerator.Current)) do
                                  yield enumerator.Current |> snd
                                  moreData := enumerator.MoveNext()
                          }
        }
    
    /// Create a key/value pair from a string.
    let intoKeyValue (line : string) = 
        let parts = line.Split('\t')
        parts.[0], parts.[1]

/// A key/value pair that represents a single output from a Map or Reduce.
type HadoopOutput = string * string

/// The input for a reducer is a key and a set of values
type ReducerInput = string * seq<string>

/// Represents the different outputs that a Map or Reducer can have.
type MRFunction<'a> = 
    /// A mapper or reducer that contains zero or one output.
    | SingleOutput of ('a -> HadoopOutput option)
    /// A mapper or reducer that contains many outputs.
    | ManyOutputs of ('a -> seq<HadoopOutput>)

let private processToOutput (action, writer) = 
    match action with
    | SingleOutput action -> Seq.choose action
    | ManyOutputs action -> Seq.collect action
    >> Seq.map (fun (key, value) -> sprintf "%s\t%s" key value)
    >> Seq.iter writer

/// Runs a mapper using a custom Reader and Writer.
let doMapCustom (reader, writer) mapper = reader |> processToOutput (mapper, writer)

/// Runs a reducer using a custom Reader and Writer.
let doReduceCustom (reader, writer) (reducer : MRFunction<ReducerInput>) = 
    reader
    |> Seq.map Helpers.intoKeyValue
    |> Helpers.groupByAdjacent
    |> processToOutput (reducer, writer)

/// Runs a mapper using the Console for IO, ideal for use with Streaming Hadoop.
let doMap = doMapCustom (Readers.Console, Writers.Console)

/// Runs a reducer using the Console for IO, ideal for use with Streaming Hadoop.
let doReduce = doReduceCustom (Readers.Console, Writers.Console)