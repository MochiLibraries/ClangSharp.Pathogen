#!/usr/bin/env python3
import hashlib
import io
import os
import platform
import stat
import sys
import tarfile
import urllib.request

import gha

# Configuration
version = "v0.3.0"
url_base = f"https://github.com/mozilla/sccache/releases/download/{version}/"

platform = f"{platform.system()}-{platform.machine()}"
if platform == 'Windows-AMD64':
    expected_hash = "f25e927584d79d0d5ad489e04ef01b058dad47ef2c1633a13d4c69dfb83ba2be"
    download_file_name = f"sccache-{version}-x86_64-pc-windows-msvc"
    binary_name = "sccache.exe"
elif platform == 'Linux-x86_64':
    expected_hash = "e6cd8485f93d683a49c83796b9986f090901765aa4feb40d191b03ea770311d8"
    download_file_name = f"sccache-{version}-x86_64-unknown-linux-musl"
    binary_name = "sccache"
elif platform == 'Linux-aarch64':
    expected_hash = "9ae4e1056b3d51546fa42a4cbf8e95aa84a4b2b4c838f9114e01b7fef5c0abd0"
    download_file_name = f"sccache-{version}-aarch64-unknown-linux-musl"
    binary_name = "sccache"
elif platform == 'Darwin-x86_64':
    expected_hash = "61c16fd36e32cdc923b66e4f95cb367494702f60f6d90659af1af84c3efb11eb"
    download_file_name = f"sccache-{version}-x86_64-apple-darwin"
    binary_name = "sccache"
elif platform == 'Darwin-arm64':
    expected_hash = "65d0a04fac51eaeeadd72d3f7eee3fdc27409aaf23b97945ea537e92bd0b0f0d"
    download_file_name = f"sccache-{version}-aarch64-apple-darwin"
    binary_name = "sccache"
else:
    assert(False), f"Unknown platform '{platform}'"

sccache_url = f"{url_base}{download_file_name}.tar.gz"
name_in_tar = f"{download_file_name}/{binary_name}"

# Figure out the sccache directory paths and create it if necessary
# We assume our working directory is the repo root.
# The binary is placed in a subdirectory of its download hash to ensure we don't re-use an old version.
sccache_root = os.path.join(os.getcwd(), "bin", "tools", "sccache")
sccache_cache_directory = os.path.join(sccache_root, "cache")
sccache_cache_log_file_path = os.path.join(sccache_root, "sccache.log")

sccache_directory = os.path.join(sccache_root, expected_hash)
sccache_binary_location = os.path.join(sccache_directory, binary_name)
os.makedirs(sccache_directory, exist_ok=True)

# Add sccache to the workspace path and configure sccache's cache directory
gha.set_output('root-directory', sccache_root)
gha.set_output('log-file-path', sccache_cache_log_file_path)
gha.set_environment_variable('SCCACHE_DIR', sccache_cache_directory)
gha.set_environment_variable('SCCACHE_ERROR_LOG', sccache_cache_log_file_path)
gha.set_environment_variable('SCCACHE_LOG', 'info')
gha.add_path(sccache_directory)

# If the output path already exists, no need to download
# (This will happen automagically because the binary is going to be cached along with the cache its self.)
if os.path.exists(sccache_binary_location):
    print("sccache already downloaded, won't download again.")
    sys.exit()

# Download the sccache archive
print(f"Download sccache from '{sccache_url}'...")
response = urllib.request.urlopen(sccache_url)
file_data = response.read()

# Validate the file hash
file_hash = hashlib.sha256(file_data).hexdigest()
assert(file_hash == expected_hash), f"Failed to validate '{sccache_url}', expected SHA256 hash {expected_hash} (got {file_hash})"

# Extract the sccache binary
tar_stream = io.BytesIO(file_data)
with tarfile.open(fileobj=tar_stream) as tar_file:
    with tar_file.extractfile(name_in_tar) as file:
        with io.open(sccache_binary_location, mode='wb') as output_file:
            output_file.write(file.read())

# Mark the binary as executable
if sys.platform != 'win32':
    current_mode = os.stat(sccache_binary_location).st_mode
    os.chmod(sccache_binary_location, current_mode | stat.S_IXUSR | stat.S_IXGRP | stat.S_IXOTH)

# Done
print("sccache downloaded.")
