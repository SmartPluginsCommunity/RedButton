using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;


namespace RedButton.Common.TeklaStructures.PileMarker
{
    internal class PileNumerator
    {
        private List<Beam> SelectedPilesToList()
        {
            var model = new Model();
            var modelObjectSelector = new TSMUI.ModelObjectSelector();
            var pileArray = modelObjectSelector.GetSelectedObjects();
            var numerator = pileArray.GetEnumerator();
            var partList = new List<Beam>();
            while (numerator.MoveNext())
            {
                var temp = numerator.Current as Beam;
                if (temp == null) continue;
                if (temp.Type == Beam.BeamTypeEnum.COLUMN)
                {
                    partList.Add(temp);
                }
            }
            if (partList.Count > 0) return partList;
            else
            {
                var ex = new Exception("PartList is Empty");
                throw ex;
            }
        }

        private void WritePileNumbers(List<Beam> pileList)
        {
            var orderedEnumerable = pileList.OrderBy(beam => beam.StartPoint.X).ThenBy(beam => beam.StartPoint.Y);
            var sortedList = orderedEnumerable.ToList();
            for (int i = 0; i < sortedList.Count(); i++)
            {
                var temp = sortedList[i];
                temp.SetUserProperty("PILE_NUMBER", i + 1.ToString());
            }

        }

        private void PileTypeMark(List<Beam> pileList)
        {
            var groups = from pile in pileList.AsEnumerable()
                         group pile by new
                         { profile = pile.Profile.ToString(),
                             length = pile.EndPoint.Z - pile.StartPoint.Z,
                             level = pile.EndPoint.Z,
                         } into gr
                         select gr;
            var enumerator = groups.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int i = 1;
                foreach (var pile in enumerator.Current)
                {
                    pile.SetUserProperty("USERDEFINED.PILE_TYPE", i.ToString());
                }
                i++;
            }
        }
    }


}
