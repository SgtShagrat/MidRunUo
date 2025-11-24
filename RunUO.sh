#!/bin/sh
if [ "$1" = "-r" ]; then
	rm Scripts/Output/*
fi

if [ "$1" = "-d" ]; then
	options="$options -debug"
fi

if [ "$1" = "-s" ]; then
	options="$options -skipcompile"
fi

mono --debug RunUO.exe $options
