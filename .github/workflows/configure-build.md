# ClangSharp.Pathogen CI Configuration Script

This script is responsible for configuring the environment to build a specific configuration of ClangSharp.Pathogen and to push packages to NuGet (if applicable)

## Inputs

Inputs are provided via environment variables. All environment variables are expected to be present.

Variable                        | Description
--------------------------------|--------------------------------------------------------------
`github_event_name` | `github.event_name`
`github_repository_owner` | `github.repository_owner`
`github_run_number` | `github.run_number`
`github_ref` | `github.ref`
`is_official_source` | `secrets.is_official_source`
`input_will_publish_packages` | `github.event.inputs.will_publish_packages`
`input_preview_release_version` | `github.event.inputs.preview_release_version`
`input_do_full_release` | `github.event.inputs.do_full_release`

## Outputs

### MSBuild Properties

The following environment variables will bet set for use by MSBuild (see `versioning.props` for details):

Variable | Description
---------|------------
`Configuration` | The project configuration that will be built.
`ContinuousIntegrationBuild` | `true`
`ContinuousIntegrationBuildKind` | The kind of build to be produced by this run.
`PreviewReleaseVersion` | The preview release version to use, if applicable.

### GitHub Step Outputs

Variable | Description
---------|------------
`publish-to-github` | `true` if packages will be published.
