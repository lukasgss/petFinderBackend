using System.Collections.Generic;
using Application.Common.Interfaces.FrontendDropdownData;

namespace Tests.EntityGenerators;

public static class DropdownDataGenerator
{
    public static DropdownDataResponse<Guid> GenerateDropdownDataResponseGuid()
    {
        return new DropdownDataResponse<Guid>()
        {
            Text = "Text",
            Value = Guid.NewGuid()
        };
    }

    public static List<DropdownDataResponse<Guid>> GenerateListDropdownDataResponseGuid()
    {
        return new List<DropdownDataResponse<Guid>>()
        {
            GenerateDropdownDataResponseGuid()
        };
    }
    
    public static DropdownDataResponse<int> GenerateDropdownDataResponseInt()
    {
        return new DropdownDataResponse<int>()
        {
            Text = "Text",
            Value = 1
        };
    }

    public static List<DropdownDataResponse<int>> GenerateListDropdownDataInt()
    {
        return new List<DropdownDataResponse<int>>()
        {
            GenerateDropdownDataResponseInt()
        };
    }
}