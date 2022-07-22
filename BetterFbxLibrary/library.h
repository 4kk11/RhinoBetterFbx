#pragma once

extern "C"
{
    int num = 0;
    __declspec(dllexport) void SetNum(int n);
    __declspec(dllexport) int GetNum();
}