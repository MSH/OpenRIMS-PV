using System.ComponentModel.DataAnnotations.Schema;

namespace OpenRIMS.PV.Main.Core.SeedWork
{
	internal interface IHasCustomAttributes
	{
		[Column(TypeName = "xml")]
		string CustomAttributesXmlSerialised { get; set; }
	}
}