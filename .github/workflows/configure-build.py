#!/usr/bin/env python3
import hashlib
import os
import re
import sys

#==================================================================================================
# Utility Functions
#==================================================================================================
errors_were_printed = False

def fail_if_errors():
    if errors_were_printed:
        print("Exiting due to previous errors.", file=sys.stderr)
        sys.exit(1)

def print_error(message):
    global errors_were_printed
    errors_were_printed = True
    print(f"::error::{message}", file=sys.stderr)

def print_warning(message):
    print(f"::warning::{message}", file=sys.stderr)

def set_environment_variable(name, value):
    print(f"::set-env name={name}::{value}")

def set_output(name, value):
    if isinstance(value, bool):
        value = "true" if value else "false"
    print(f"::set-output name={name}::{value}")

#==================================================================================================
# Get inputs
#==================================================================================================
def get_environment_variable(name):
    ret = os.getenv(name)

    if ret is None:
        print_error(f"Missing required parameter '{name}'")
    
    if (ret == ''):
        return None

    return ret

github_event_name = get_environment_variable('github_event_name')
github_repository_owner = get_environment_variable('github_repository_owner')
github_run_number = get_environment_variable('github_run_number')
github_ref = get_environment_variable('github_ref')

# This is obtuse because you shouldn't be trying to set this for forks of this repository!
is_official_source = get_environment_variable('is_official_source') or ''
is_official_source_hash = hashlib.sha256(is_official_source.encode()).hexdigest()
is_official_source = is_official_source_hash == '2979f945cc91048fcc6a18abfb47b95727b32ca688371882f5b900f55f2bf87a'

will_publish_packages = False
is_preview_release = False
preview_release_version = f"invalid{github_run_number}"
is_full_release = False

if github_event_name == 'push':
    # Publish packages if the push is to the main branch
    will_publish_packages = github_ref == 'refs/heads/main'
elif github_event_name == 'pull_request':
    # Never publish packages for pull requests
    will_publish_packages = False
elif github_event_name == 'workflow_dispatch':
    # Publish packages if enabled by the user
    will_publish_packages = get_environment_variable('input_will_publish_packages') == 'true'
    
    preview_release_version = get_environment_variable('input_preview_release_version')
    is_preview_release = preview_release_version != None

    is_full_release = get_environment_variable('input_do_full_release') == 'true'

    if is_preview_release and re.match('^[0-9a-zA-Z.-]+$', preview_release_version) is None:
        print_error(f"'{preview_release_version}' is not a valid preview release identifier.")
    
    if is_full_release and is_preview_release:
        print_error(f"A release cannot be both a full release and a preview release.")
    
    if is_full_release and not is_official_source:
        print_warning("Full release should probably not be created by third parties.")
else:
    print_warning(f"Unexpected GitHub event '{github_event_name}'")

# If there are any errors at this point, make sure we exit with an error code
fail_if_errors()

#==================================================================================================
# Emit MSBuild Properties
#==================================================================================================
set_environment_variable('ContinuousIntegrationBuild', 'true')

if is_preview_release:
    set_environment_variable('Configuration', 'Release')
    set_environment_variable('ContinuousIntegrationBuildKind', 'PreviewRelease')
    set_environment_variable('PreviewReleaseVersion', preview_release_version)
elif is_full_release:
    set_environment_variable('Configuration', 'Release')
    set_environment_variable('ContinuousIntegrationBuildKind', 'FullRelease')
else:
    set_environment_variable('Configuration', 'Debug')
    set_environment_variable('ContinousIntegrationRunNumber', github_run_number)

# If this build is happening outside of InfectedLibraries, add a fork name
if not is_official_source:
    set_environment_variable('ForkName', github_repository_owner)

#==================================================================================================
# Emit step outputs
#==================================================================================================
set_output('publish-to-github', will_publish_packages)

#==================================================================================================
# Final check to exit with an error code if any errors were printed
#==================================================================================================
fail_if_errors()
