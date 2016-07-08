using Essence.Comms;
using Essence.Dto;
using Essence.Global;
using Essence.Injection;
using Essence.IOC;
using Essence.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xciles.PclValueInjecter;
using Essence.Model;
using Essence.Model.Model;

namespace Essence.ViewModel
{
    public class SeriesFavoritesCollection : ObservableCollection<SeriesFavoritesModel>
    {
        public void CopyFrom(IEnumerable<SeriesFavoritesDTO> SeriesFavoritesDTOs)
        {
            this.Items.Clear();
            foreach (var SeriesFavoritesDTO in SeriesFavoritesDTOs)
            {
                var SerieModel = DtoToModel(SeriesFavoritesDTO);
                this.Items.Add(SerieModel);
                ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Add<string, SerieModel>(SeriesFavoritesDTO.imdbID, (SerieModel)SerieModel, DateTime.Now.AddDays(1));
            }
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(SeriesFavoritesDTO SeriesFavoritesDTO)
        {
            this.Items.Add(DtoToModel(SeriesFavoritesDTO));
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private SeriesFavoritesModel DtoToModel(SeriesFavoritesDTO SeriesFavoritesDTO)
        {
            SeriesFavoritesModel SeriesFavoritesModel = new SeriesFavoritesModel();
            SeriesFavoritesModel.InjectFrom(SeriesFavoritesDTO);
            return SeriesFavoritesModel;
        }

        public async Task Get(IWindow EssenceWindows)
        {
            using (var SeriesFavoritesRepository = new SeriesFavoritesRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    IEnumerable<SeriesFavoritesDTO> response = await SeriesFavoritesRepository.GetSeriesFavoritesAsync();
                    this.CopyFrom(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SeriesFavoritesRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar SeriesFavorites"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }


        public async Task CargaEnCacheSerieById(IWindow EssenceWindows,string imdbID)
        {
            SerieCollection SerieCollection = new SerieCollection();
            await SerieCollection.GetById(EssenceWindows,imdbID);

            return;
        }


        public async Task Post(IWindow EssenceWindows, SerieModel SerieModel)
        {
            using (var SeriesFavoritesRepository = new SeriesFavoritesRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    SeriesFavoritesDTO SeriesFavoritesDTO = new SeriesFavoritesDTO();
                    SeriesFavoritesDTO.InjectFrom(SerieModel);
                    SeriesFavoritesDTO response = await SeriesFavoritesRepository.PostSeriesFavoritesAsync(SeriesFavoritesDTO);
                    SerieModel.InjectFrom(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.SeriesFavorites, Essence.Enums.ModelsEnum.TiposMetodos.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SeriesFavoritesRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar SeriesFavorites"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }

        public async Task Delete(IWindow EssenceWindows, SerieModel SerieModel)
        {
            using (var SeriesFavoritesRepository = new SeriesFavoritesRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    SeriesFavoritesDTO SeriesFavoritesDTO = new SeriesFavoritesDTO();
                    SeriesFavoritesDTO.InjectFrom(SerieModel);
                    await SeriesFavoritesRepository.DeleteMemberAsync(SerieModel.Id);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.SeriesFavorites, Essence.Enums.ModelsEnum.TiposMetodos.Delete);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SeriesFavoritesRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar SeriesFavorites"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }

    }
}
