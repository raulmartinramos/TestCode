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

namespace Essence.ViewModel
{
    public class SerieCollection : ObservableCollection<SerieModel>
    {
        public void CopyFrom(IEnumerable<SerieDTO> SerieDTOs)
        {
            this.Items.Clear();
            foreach (var SerieDTO in SerieDTOs)
            {
                var SerieModel = DtoToModel(SerieDTO);
                FiltroSerieModel(SerieModel);
                Cache(SerieModel);
                this.Items.Add(SerieModel);
            }
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(SerieDTO SerieDTO)
        {
            var SerieModel = DtoToModel(SerieDTO);
            FiltroSerieModel(SerieModel);
            Cache(SerieModel);
            this.Items.Add(SerieModel);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(SerieModel SerieModel)
        {
            FiltroSerieModel(SerieModel);
            Cache(SerieModel);
            this.Items.Add(SerieModel);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        private void Cache(SerieModel SerieModel)
        {
            var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(SerieModel.imdbID);
            if (SerieModelCacheada == null)
                ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Add<string, SerieModel>(SerieModel.imdbID, SerieModel, DateTime.Now.AddDays(1));
        }

        private void FiltroSerieModel(SerieModel SerieModel)
        {
            if (!string.IsNullOrEmpty(SerieModel.Season))
            {
                var favoritas = ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites;
                var a = favoritas.Count();
                var SeriesFavorites = ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites.Where(c => c.imdbID == SerieModel.imdbID).FirstOrDefault();
                if (SeriesFavorites != null)
                {
                    SerieModel.Id = SeriesFavorites.Id;
                    SerieModel.Version = SeriesFavorites.Version;
                    SerieModel.Favorite = true;
                }
            }

            if (!string.IsNullOrEmpty(SerieModel.Episode))
            {
                var EpisodieRating = ServiceContainer.Resolve<GlobalDataFrameWork>().EpisodiesRating.Where(c => c.imdbID == SerieModel.imdbID).FirstOrDefault();
                if (EpisodieRating != null)
                {
                    SerieModel.Id = EpisodieRating.Id;
                    SerieModel.Version = EpisodieRating.Version;
                    SerieModel.Rating = EpisodieRating.Rating;
                }
            }

        }

        private SerieModel DtoToModel(SerieDTO SerieDTO)
        {
            SerieModel SerieModel = new SerieModel();
            SerieModel.InjectFrom(SerieDTO);
            return SerieModel;
        }

        public async Task Get(IWindow EssenceWindows, string Title)
        {
            using (var SerieRepository = new SerieRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    IEnumerable<SerieDTO> response = await SerieRepository.GetSerieAsyncByTitle(Title);
                    this.CopyFrom(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SerieRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Serie"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }

        public async Task GetById(IWindow EssenceWindows, string imdbID)
        {
            using (var SerieRepository = new SerieRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    SerieDTO response = await SerieRepository.GetSerieAsyncById(imdbID);
                    this.Add(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SerieRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Serie"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }

        public async Task GetBySearch(IWindow EssenceWindows, SearchModel SearchModel)
        {
            using (var SerieRepository = new SerieRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    foreach (var serie in SearchModel.Search)
                    {
                        var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(serie.imdbID);
                        if (SerieModelCacheada == null)
                        {
                            SerieDTO response = await SerieRepository.GetSerieAsyncById(serie.imdbID);
                            SerieModelCacheada = DtoToModel(response);
                            ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Add<string, SerieModel>(serie.imdbID, SerieModelCacheada, DateTime.Now.AddDays(1));
                        }
                        this.Add(SerieModelCacheada);
                        EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie);
                    }
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SerieRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Serie"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }


        public async Task GetSeasonsById(IWindow EssenceWindows, string imdbID)
        {
            using (var SerieRepository = new SerieRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    this.Clear();
                    var TemporadasCacheadas = GetCacheSeasonsById(imdbID);
                    var SerieModel = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(imdbID);
                    int seasoncounter = 1;
                    bool therearemoreseasons = true;
                    while (therearemoreseasons)
                    {
                        SerieModel SerieModelCacheada = TemporadasCacheadas.Where(c => c.Season == seasoncounter.ToString()).FirstOrDefault();
                        if (SerieModelCacheada == null)
                        {
                            SerieDTO response = await SerieRepository.GetSerieAsyncByTemporada(SerieModel.Title, seasoncounter.ToString(), "1");
                            if (response.imdbID != null)
                            {
                                SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(response.imdbID);
                                if (SerieModelCacheada == null)
                                {
                                    SerieModelCacheada = DtoToModel(response);
                                    ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Add<string, SerieModel>(SerieModelCacheada.imdbID, SerieModelCacheada, DateTime.Now.AddDays(1));
                                }
                            }
                            else
                                therearemoreseasons = false;
                        }
                        this.Add(SerieModelCacheada);
                        EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie);
                        seasoncounter++;
                    }
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SerieRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Serie"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }


        public async Task GetEpisodiesById(IWindow EssenceWindows, string imdbID, string Season)
        {
            using (var SerieRepository = new SerieRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    this.Clear();
                    var SerieModel = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(imdbID);
                    var EpisodiossCacheados = GetCacheEpisodiesById(imdbID, Season);
                    if (EpisodiossCacheados.Count() <= 1) //Siempre tiene 1 utilizado para la temporada inexistente en B.D.
                    {
                        int episodiecounter = 1;
                        bool therearemoreepisodies = true;
                        while (therearemoreepisodies)
                        {
                            SerieDTO response = await SerieRepository.GetSerieAsyncByTemporada(SerieModel.Title, Season, episodiecounter.ToString());
                            if (response.imdbID != null)
                            {
                                var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(response.imdbID);
                                if (SerieModelCacheada == null)
                                {
                                    SerieModelCacheada = DtoToModel(response);
                                    ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Add<string, SerieModel>(SerieModelCacheada.imdbID, SerieModelCacheada, DateTime.Now.AddDays(1));
                                }
                                this.Add(SerieModelCacheada);
                                EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Espisodies);
                                episodiecounter++;
                            }
                            else
                                therearemoreepisodies = false;

                        }
                    }
                    else
                        foreach (var episodio in EpisodiossCacheados)
                            this.Add(episodio);

                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Espisodies);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SerieRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Serie"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }

        //Devuelve las temporadas o episodios primeros de una serie que se tengan en la cache
        private IEnumerable<SerieModel> GetCacheSeasonsById(string imdbID)
        {
            IList<SerieModel> Seasons = new List<SerieModel>();
            var imdbIDCacheadas = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Keys<string>();
            foreach (var keyimdbID in imdbIDCacheadas)
            {
                var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(keyimdbID);
                if (SerieModelCacheada != null)
                {
                    if (SerieModelCacheada.seriesID == imdbID && SerieModelCacheada.Episode == "1")
                        Seasons.Add(SerieModelCacheada);
                }
            }
            return Seasons;
        }

        private IEnumerable<SerieModel> GetCacheEpisodiesById(string imdbID, string season)
        {
            IList<SerieModel> Episodies = new List<SerieModel>();
            var imdbIDCacheadas = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Keys<string>();
            foreach (var keyimdbID in imdbIDCacheadas)
            {
                var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(keyimdbID);
                if (SerieModelCacheada != null)
                {
                    if (SerieModelCacheada.seriesID == imdbID && SerieModelCacheada.Season == season)
                        Episodies.Add(SerieModelCacheada);
                }
            }
            return Episodies;
        }


    }
}
