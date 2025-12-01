# Consul environment manager

This is a handy tool for fast switching between multiple configured local [Consul](https://www.consul.io/) environments.
A special configuration file `clem.yaml` will be created in user directory `%HOMEDRIVE%%HOMEPATH%`on the first run (check [example.yaml](/sources/example.yaml) for example). In addition, tool will ask for a working folder if it was not specified in the configuration file. 


Available commands:

| Command                        | Description                                                            |
|--------------------------------|------------------------------------------------------------------------|
| list                           | Print available environment list.                                      |
| list `alias`                   | Print specified environment versions.                                  |
| switch                         | Switch to the latest environment version.                              |
| switch `alias`                 | Switch to specified environment version.                               |
| clone `alias`                  | Clone defined remote environment.                                      | 
| add `alias` `endpoint`         | Save remote environment without authorization in configuration.        |
| add `alias` `endpoint` `token` | Save remote environment with authorization via token in configuration. |
| backup                         | Backup local environment configuration.                                |
| drop                           | Drop local environment configuration.                                  |


The simplest way to back up your local consul environment is to run the following command:
```
clem backup
```
This command will save your configuration in a reserved folder named `backup` in your working directory. 


Let's imagine that you need to restore the configuration from a specific version. For this result, you need to run the following command:
```
clem switch UAT31 1764422923
```
In this case, `UAT31` is an alias for you known environment, and `1764422923` is his version. You can also easily restore the previous version for your local environment with the following command:
```
clem switch backup
```
If you leave the version field blank, then the latest one will be automatically selected. The latest version of the backup is being created immediately before applying the new configuration to your local environment, so you don't have to worry about losing any information.


