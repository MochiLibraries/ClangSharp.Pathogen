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
version = "v0.2.15"
url_base = f"https://github.com/mozilla/sccache/releases/download/{version}/"

platform = f"{platform.system()}-{platform.machine()}"
if platform == 'Windows-AMD64':
    expected_hash = "3dfecdbb85561c55e899d3ad039c671f806d283c49da0721c2ef5c1310d87965"
    download_file_name = f"sccache-{version}-x86_64-pc-windows-msvc"
    binary_name = "sccache.exe"
elif platform == 'Linux-x86_64':
    expected_hash = "e5d03a9aa3b9fac7e490391bbe22d4f42c840d31ef9eaf127a03101930cbb7ca"
    download_file_name = f"sccache-{version}-x86_64-unknown-linux-musl"
    binary_name = "sccache"
elif platform == 'Linux-aarch64':
    expected_hash = "90d91d21a767e3f558196dbd52395f6475c08de5c4951a4c8049575fa6894489"
    download_file_name = f"sccache-{version}-aarch64-unknown-linux-musl"
    binary_name = "sccache"
elif platform == 'Darwin-x86_64':
    expected_hash = "908e939ea3513b52af03878753a58e7c09898991905b1ae3c137bb8f10fa1be2"
    download_file_name = f"sccache-{version}-x86_64-apple-darwin"
    binary_name = "sccache"
elif platform == 'Darwin-arm64':
    expected_hash = "4120626b3a13b8e615e995b926db4166dc2b34274908b8f159ca65be4928b32a"
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
