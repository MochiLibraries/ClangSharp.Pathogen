#!/usr/bin/env python3
import os
import re

import gha

#==================================================================================================
# Get inputs
#==================================================================================================
def get_environment_variable(name):
    ret = os.getenv(name)

    if ret is None:
        gha.print_error(f"Missing required parameter '{name}'")
    
    if (ret == ''):
        return None

    return ret

github_event_name = get_environment_variable('github_event_name')
github_run_number = get_environment_variable('github_run_number')

#==================================================================================================
# Determine build settings
#==================================================================================================

version = f'0.0.0-ci{github_run_number}'
configuration = 'Debug'

if github_event_name == 'release':
    version = get_environment_variable('release_version')
    configuration = 'Release'
elif github_event_name == 'workflow_dispatch':
    workflow_dispatch_version = get_environment_variable('workflow_dispatch_version')
    if workflow_dispatch_version is not None:
        version = workflow_dispatch_version
        configuration = 'Release'

# Trim leading v off of version if present
if version.startswith('v'):
    version = version[1:]

# Validate the version number
if not re.match('^\d+\.\d+\.\d+(-[0-9a-zA-Z.-]+)?$', version):
    gha.print_error(f"'{version}' is not a valid semver version!")

# If there are any errors at this point, make sure we exit with an error code
gha.fail_if_errors()

#==================================================================================================
# Emit MSBuild properties
#==================================================================================================
print(f"Configuring build environment to build {configuration.lower()} @ {version}")
gha.set_environment_variable('Configuration', configuration)
gha.set_environment_variable('CiBuildVersion', version)

#==================================================================================================
# Final check to exit with an error code if any errors were printed
#==================================================================================================
gha.fail_if_errors()
