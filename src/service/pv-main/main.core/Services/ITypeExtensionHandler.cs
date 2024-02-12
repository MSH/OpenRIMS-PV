using System.Collections.Generic;
using OpenRIMS.PV.Main.Core.CustomAttributes;

namespace OpenRIMS.PV.Main.Core.Services
{
    public interface ITypeExtensionHandler
    {
        /// <summary>
        /// Returns a list of unpopulated Custom Attributes for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<CustomAttributeDetail> BuildModelExtension<T>() where T : IExtendable;

        /// <summary>
        /// Returns list of Custom Attributes for the specified extendable object with the values prepopulated
        /// </summary>
        /// <param name="extendableObject">The extendable object.</param>
        /// <returns></returns>
        List<CustomAttributeDetail> BuildModelExtension(IExtendable extendableObject);

        /// <summary>
        /// Updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        IExtendable UpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser);

        /// <summary>
        /// Validates and updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        IExtendable ValidateAndUpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser);
    }
}
