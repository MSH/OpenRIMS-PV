using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.SeedWork
{
	internal interface IHasCustomAttributes
	{
		[Column(TypeName = "xml")]
		string CustomAttributesXmlSerialised { get; set; }
	}
}