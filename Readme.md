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
