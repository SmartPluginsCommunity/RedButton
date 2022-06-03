using RedButton.Common.Core.CollectionExtensions;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Catalogs;

namespace RedButton.Common.TeklaStructures.Catalogs
{
    public static class Catalogs
    {
        /// <summary>
        /// Получаем список пользовательских атрибутов в зависимоти от типа элемента.
        /// </summary>
        /// <param name="typeElements"></param>
        public static IEnumerable<string> GetUserPropertyItemsForm(CatalogObjectTypeEnum typeElements)
        {
            return new CatalogHandler()
                .GetUserPropertyItems(typeElements)
                .ToIEnumerable<UserPropertyItem>()
                .Select(item => item.Name)
                .OrderBy(name => name);
        }

        /// <summary>
        /// Класс получает список стандарттов болтов из открытой модели текла.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> BoltStandart()
        {
            return new CatalogHandler()
            .GetBoltItems()
            .ToIEnumerable<BoltItem>()
            .Select(bolt => bolt.Standard)
            .Distinct();
        }

        /// <summary>
        /// Метод получает набор диаметров болтов по указанному стандарту болтов.
        /// </summary>
        /// <param name="boltStandart"></param>
        /// <returns></returns>
        public static IEnumerable<double> BoltSize(string boltStandart)
        {
            return new CatalogHandler()
                .GetBoltItems()
                .ToIEnumerable<BoltItem>()
                .Where(bolt => bolt.Standard == boltStandart)
                .Select(bolt => bolt.Size);
        }

        /// <summary>
        /// Получаем список профилей по типу. Бетон, металл, дерево.
        /// </summary>
        /// <param name="profileType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetProfile(ProfileItem.ProfileItemTypeEnum profileType)
        {
            return new CatalogHandler()
                .GetProfileItems()
                .ToIEnumerable<LibraryProfileItem>()
                .Where(profile => profile.ProfileItemType == profileType)
                .Select(profile => profile.ProfileName);
        }

        /// <summary>
        /// Получаем список материалов по типу. Бетон, металл, дерево.
        /// </summary>
        /// <param name="materialType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMaterial(MaterialItem.MaterialItemTypeEnum materialType)
        {
            return new CatalogHandler()
                .GetMaterialItems()
                .ToIEnumerable<MaterialItem>()
                .Where(material => material.Type == materialType)
                .Select(material => material.MaterialName);
        }

        /// <summary>
        /// Получаем список всех материалов.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GelAllMaterial()
        {
            return new CatalogHandler()
                .GetMaterialItems()
                .ToIEnumerable<MaterialItem>()
                .Select(material => material.MaterialName);
        }

        /// <summary>
        /// Получаем список префиксов для профилей по типу.
        /// </summary>
        /// <param name="profileType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPrefixProfile(ProfileItem.ProfileItemTypeEnum profileType)
        {
            return new CatalogHandler()
                .GetProfileItems()
                .ToIEnumerable<ParametricProfileItem>()
                .Where(item => item.ProfileItemType == profileType)
                .Select(item => item.ProfilePrefix)
                .OrderBy(prefix => prefix);
        }
    }
}
