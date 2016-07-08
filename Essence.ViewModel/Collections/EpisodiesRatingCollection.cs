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
using Essence.Model.Model;

namespace Essence.ViewModel
{
    public class EpisodiesRatingCollection : ObservableCollection<EpisodiesRatingModel>
    {
        public void CopyFrom(IEnumerable<EpisodiesRatingDTO> EpisodiesRatingDTOs)
        {
            this.Items.Clear();
            foreach (var EpisodiesRatingDTO in EpisodiesRatingDTOs)
                this.Items.Add(DtoToModel(EpisodiesRatingDTO));
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(EpisodiesRatingDTO EpisodiesRatingDTO)
        {
            this.Items.Add(DtoToModel(EpisodiesRatingDTO));
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private EpisodiesRatingModel DtoToModel(EpisodiesRatingDTO EpisodiesRatingDTO)
        {
            EpisodiesRatingModel EpisodiesRatingModel = new EpisodiesRatingModel();
            EpisodiesRatingModel.InjectFrom(EpisodiesRatingDTO);
            return EpisodiesRatingModel;
        }

        public async Task Get(IWindow EssenceWindows)
        {
            using (var EpisodiesRatingRepository = new EpisodiesRatingRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    IEnumerable<EpisodiesRatingDTO> response = await EpisodiesRatingRepository.GetEpisodiesRatingAsync();
                    this.CopyFrom(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, EpisodiesRatingRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar EpisodiesRating"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }



        public async Task Post(IWindow EssenceWindows, EpisodiesRatingDTO EpisodiesRatingDTO)
        {
            using (var EpisodiesRatingRepository = new EpisodiesRatingRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    EpisodiesRatingDTO response = await EpisodiesRatingRepository.PostEpisodiesRatingAsync(EpisodiesRatingDTO);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.EpisodiesRating, Essence.Enums.ModelsEnum.TiposMetodos.Post);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, EpisodiesRatingRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar EpisodiesRating"));
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
