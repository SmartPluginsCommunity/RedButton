using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RedButton.Common.Core.RevitCore.Interfaces
{
    public interface IRevitCore
    {
        UIApplication UIApplication { get; set; }
        UIControlledApplication UIControlledApplication { get;}
        ControlledApplication ControlledApplication { get; }
        Document Document { get; set; }
    }
}