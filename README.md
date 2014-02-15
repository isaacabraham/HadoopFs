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
