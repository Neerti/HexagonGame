#!/bin/bash
dotnet publish -c Release -r linux-x64 -o artifacts/linux --self-contained
