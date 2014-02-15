HadoopFs
========

The existing .NET APIs for Hadoop that runs on e.g. HDInsight are somewhat awkward from an F# point of view: -

- Requires inheriting from abstract classes
- Inpure style makes code harder to test

HadoopFs makes life easier for the F# developer that wants to develop map/reduce jobs: -

- No base class hierarchy for your map / reduce functions to adhere to
- Support for both optional single-instance outputs and output collections
- Support for easily testing inputs and outputs from file system as well as in-memory data source, or you can supply your own
- Easy to test your map and reduce functions - no need for external Hadoop test libraries

## Basic structure

Mappers and reducers have no restrictions in their makeup e.g. method, base class, function etc. - they simply must have one of the following two signatures

    // mappers
    string -> (string * string) option
    string -> (string * string) seq
    
    // reducers
    string * (string seq) -> (string * string) option
    string * (string seq) -> (string * string) seq

The former is one that will normally be used, but it is sometimes necessary to return multiple outputs from a single call to a mapper or reducer.

### Examples
    // map the length of the line passed in for long lines
    // e.g. "isaac" -> ("5", "1")
    let mapLineLength line = 
        match line |> Seq.length with
        | len when len > 10 -> Some((line |> Seq.length).ToString(), "1")
        | _ -> None
    
    // Take in the outputs from mapLineLength after grouping on the key 
    let reduceStringLength key values =
        Some(key, (values |> Seq.length).ToString())
        
You can also return sequences from a mapper or reducer - this is common when expanding a single line to multiple results e.g. Word Count.

### Using within Hadoop
Currently you have to create a single executable for each mapper and reducer, which is what Hadoop uses to communicate with via the Console input and output streams. To tie the above map and reducer functions with an executable, simply create a new F# Console application: -

    open HadoopFs
    open HadoopFs.Core
    open HadoopFs.Samples

    [<EntryPoint>]
    let mainMap argv =
        doMap <| MultiValue WordCount.Mapper
        0

    [<EntryPoint>]
    // A reducer exe
    let mainReduce argv =
        doReduce <| SingleValue WordCount.Reducer
        0

The use of SingleValue and MultiValue are how you indicate to the HadoopFS runner whether the your Mapper and Reducer functions return single items or multiple items.

### Testing
Testing individual map and reduce function in isolation is simple, as you are just using regular F# functions, so you can call the above examples function directly to test the outputs. To test with larger outputs, you have several options: -

#### Testing mappers or reducers in isolation
You can can the same code as the doMap / doRecuce functions above, except you can specify the input and output "streams". There are several supplied already e.g. File System, Console and in-memory collections: -

    // Do the word count mapper, reading the input from the file system and outputting the results to the console
    doMapCustom(MultiValue WordCount.Mapper, IO.Readers.FileSystem(@"C:\Test.tsv"), IO.Writers.Console)
    
In addition, you can test out a full map-reduce: -

    let data = ["first line"; "second line"; "third line" ]
    let reducedOutput = doInMemoryMapReduce(MultiValue WordCount.Mapper, SingleValue WordCount.Reducer, data)
    // reducedOutput = seq [("first", "1"); ("line", "3"); ("second", "1"); ("third", "1")]
