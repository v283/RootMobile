// Plan (pseudocode):
// 1. The compiler error CS0051 indicates that a public member (the constructor
//    ShowPinDetailPopup.ShowPinDetailPopup(PlantPinData)) has a parameter type
//    'PlantPinData' that is less accessible than the constructor.
// 2. To fix this, make the 'PlantPinData' type at least as accessible as the
//    constructor that uses it. The simplest and most compatible fix is to
//    declare 'PlantPinData' as public.
// 3. Create/update the file that contains the PlantPinData type and mark the
//    class as public, preserving all existing properties.
// 4. Place the class in a sensible namespace (matching project layout).
//
// Implementation:
// - Provide a public class 'PlantPinData' with the same auto-properties shown in
//   the provided type signatures. This resolves the accessibility mismatch.

using System;

namespace RootMobile.Models
{
    public class PlantPinData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}