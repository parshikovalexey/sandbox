# Migration tool Example

The example demonstrate the tool to migrate several objects from one source to another source (For example move objects from one Database to another one). The purpose of the tool - allow to build hierarchically tree of the complex object to migrate them from one source to another one and normalize object identifiers even they were changed in the target source. 


# Important notes

The tool implements some general example of some models and doesn't contain implementation for some external services, like **IDataService** or **IDataSourceAdapter**. Such services should be implemented according to the used source and target storage and injected before the migration process started. Also this examples doesn't implement the process of the Migration queue creation, due to it can be so different.