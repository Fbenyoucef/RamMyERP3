using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using RamMyERP3.Models;
using RamMyERP3.DataContext;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace RamMyERP3.Controllers
{
    public class RamsController : Controller
    {
        private readonly MyContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        public RamsController(MyContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _appEnvironment = hostingEnvironment;
        }

        #region Lister
        // GET: Rams
        public async Task<IActionResult> Lister()
        {
            // Récupérer les données de la liste
            var Liste = _context.Ram
                .Include(d => d.collaborateur);

            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";

            // Afficher la vue
            return View(await Liste.ToListAsync());
        }

        #endregion Lister

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Récuperer les informations relatives au ram.
            Ram ram = await GetInfoRam(id);

            if (ram == null)
            {
                return NotFound();
            }

            ram.DetailsAbsence = new Dictionary<string, List<string>>();
            // recuperer la liste des presences pour le ram selectionné 
            foreach (var item in ram.ListeRamDetailsPresence)
            {
                if (!ram.DetailsAbsence.ContainsKey(item.affaireCollaborateur.affaire.NOM))
                    ram.DetailsAbsence.Add(item.affaireCollaborateur.affaire.NOM, new List<string> { item.DATE_TRAVAILLE.Day + "_" + item.NOMBREHEURE });

                else
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM].Add(item.DATE_TRAVAILLE.Day + "_" + item.NOMBREHEURE);

            }
            //recuperer la liste des absences pour le ram selectionné.
            foreach (var item in ram.ListeRamDetailsAbsence)
            {
                if (!ram.DetailsAbsence.ContainsKey(item.r_absence_type.NOM))
                {
                    ram.DetailsAbsence.Add(item.r_absence_type.NOM, new List<string> { item.DATE_ABSENCE.Day + "_" + item.NOMBREHEURES });
                }
                else
                {
                    ram.DetailsAbsence[item.r_absence_type.NOM].Add(item.DATE_ABSENCE.Day + "_" + item.NOMBREHEURES);
                }
            }
            // affectation du mode de la vue pour bloquer les inputs 
            ViewData["mode"] = "update";
            return View(ram);
        }

        //recuperation des informations relatives au RAM
        private async Task<Ram> GetInfoRam(int? id)
        {
            return await _context.Ram
                .Include(d => d.collaborateur)
                .ThenInclude(d => d.ListeAffaireCollaborateur)
                 .Include(r => r.ListeRamDetailsPresence)
                    .ThenInclude(c => c.affaireCollaborateur)
                    .ThenInclude(c => c.affaire)
                    .ThenInclude(c => c.AFFAIRETYPE)
                .Include(r => r.ListeRamDetailsAbsence)
                    .ThenInclude(ListeRamDetailsAbsence => ListeRamDetailsAbsence.r_absence_type)

                .FirstOrDefaultAsync(m => m.ID == id);
        }
        #endregion


        #region Ajout

        //  Action d'affichage de la vue saisie RAM lors du clique sur Ajouter (Http Get)
        public async Task<IActionResult> CreationRam(Ram? rams)
        {
            Ram ram = new Ram();
            ram.collaborateur = _context.Collaborateur.Where(e => e.ID == 2).
                Include(d => d.ListeAffaireCollaborateur)
                   .ThenInclude(c => c.affaire)
                   .ThenInclude(c => c.AFFAIRETYPE)
                .FirstOrDefault();
            ram.collaborateurID = 2;
            if (rams.ANNEEMOIS != null)
                ram.ANNEEMOIS = rams.ANNEEMOIS;

            if (ram == null)
            {
                return NotFound();
            }
            //Affectation du mode pour debloquer les champs de saisie du RAM 
            ViewData["mode"] = "ajout";
            return View(ram);
        }
        // Valider la creation du RAM 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreationRam(string[] tableauRam, Ram ram)
        {
            // verification de la saisie du model
            if (ModelState.IsValid)
            {// model valide
                //mise à jours du Ram et insertion dans la base de données 
                var ram2 = await InsererRam(tableauRam, ram);
                return RedirectToAction(nameof(Details), ram2);
            }
            // model non valide, affichage des erreur sur la page 
            return await CreationRam(ram);

        }
        //Methode d'insertion du ram dans la base de données 
        private async Task<Ram> InsererRam(string[] tableauRam, Ram ram)
        {
            double _NombreJoursTravaille = 0;
            double _NombreJourAbsence = 0;
            var _anneeMois = ram.ANNEEMOIS.Split('-');
            int _Annee = int.Parse(_anneeMois[0]);
            int _Mois = int.Parse(_anneeMois[1]);
            ram.MOIS = _Mois;
            ram.ANNEE = _Annee;
            //Ram ramFinal = new Ram();
            //ramFinal = ram;
            //initialisation des listes d'absence et de presence 
            ram.ListeRamDetailsAbsence = new Collection<RamDetailsAbsence>();
            ram.ListeRamDetailsPresence = new Collection<RamDetailsPresence>();
            //Parcour du tableau pour determiner le nombre de jours d'absence, et le nombre de jours travaillés 
            foreach (var item in tableauRam)
            {
                //recuperer les informations saisies dans le tableau
                var tab = item.Split('_');
                var _TypeMission = tab[0];
                var _TypeAffaire = tab[1];
                var _JourConcerne = tab[2];
                var _NombreHeureTravaille = tab[3];
                ram.MOIS = _Mois;
                ram.ANNEE = _Annee;
                if (_NombreHeureTravaille != "0")
                {
                    // verifier quel type de saisie : presence ou absence 
                    switch (_TypeMission)
                    {
                        case "PRES": // presence
                            RamDetailsPresence _presence = new RamDetailsPresence();
                            var affairecollaborateuriD = _context.AffaireCollaborateur.Where(d => d.AFFAIREID == int.Parse(_TypeAffaire)).FirstOrDefault();

                            _presence.AffaireCollaborateurID = affairecollaborateuriD.ID;
                            _presence.RAMID = ram.ID;
                            _presence.DATE_TRAVAILLE = new DateTime(_Annee, _Mois, int.Parse(_JourConcerne));
                            _presence.NOMBREHEURE = double.Parse(_NombreHeureTravaille);
                            _NombreJoursTravaille += double.Parse(_NombreHeureTravaille);
                            ram.ListeRamDetailsPresence.Add(_presence);
                            break;

                        case "ABSE": //Absence
                            RamDetailsAbsence _absence = new RamDetailsAbsence();
                            _absence.DATE_ABSENCE = new DateTime(_Annee, _Mois, int.Parse(_JourConcerne));
                            _absence.R_absence_typeID = int.Parse(_TypeAffaire);
                            _absence.RAMID = ram.ID;
                            _absence.NOMBREHEURES = double.Parse(_NombreHeureTravaille);
                            //  _context.Add(_absence);
                            ram.ListeRamDetailsAbsence.Add(_absence);
                            _NombreJourAbsence += double.Parse(_NombreHeureTravaille);

                            break;
                    }
                    //   await _context.SaveChangesAsync();
                    // mise a jours du context 

                }
            }
            //mise a jours du nombre de jours travaillée et les jours d'absence dans l'objet RAM
            ram.JOURS_ABSENCE = _NombreJourAbsence;
            ram.JOURS_TRAVAILLES = _NombreJoursTravaille;
            ram.collaborateur = null;
            // mise a jours du context 
            _context.Add(ram);
            _context.Entry(ram).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return ram;
        }

        #endregion

        #region Modification
        [HttpGet]
        public async Task<IActionResult> Modifier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ram = await GetInfoRam(id);
            if (ram == null)
            {
                return NotFound();
            }
            ram.DetailsAbsence = new Dictionary<string, List<string>>();
            //Recuperation de la liste du détails des presences 
            foreach (var item in ram.ListeRamDetailsPresence)
            {
                try
                {
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM].Add("PRES_" + item.affaireCollaborateur.AFFAIREID + "_" + item.DATE_TRAVAILLE.Day + "_" + item.NOMBREHEURE);
                }
                catch
                {
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM] = new List<string> { "PRES_" + item.affaireCollaborateur.AFFAIREID + "_" + item.DATE_TRAVAILLE.Day + "_" + item.NOMBREHEURE };
                }
            }
            //Recuperation de la liste du des affaires du collaborateur 
            foreach (var item in ram.collaborateur.ListeAffaireCollaborateur)
            {
                if (item.affaire != null)
                {
                    if (!ram.DetailsAbsence.ContainsKey(item.affaire.NOM))
                    {
                        ram.DetailsAbsence[item.affaire.NOM] = new List<string> { "PRES_" + item.AFFAIREID + "_" + " " + "_" + 0 };
                    }
                }

            }
            //Recuperation de la liste du détails des absences  
            foreach (var item in ram.ListeRamDetailsAbsence)
            {
                try
                {
                    ram.DetailsAbsence[item.r_absence_type.NOM].Add("ABSE_" + item.r_absence_type.ID + "_" + item.DATE_ABSENCE.Day + "_" + item.NOMBREHEURES);
                }
                catch
                {
                    ram.DetailsAbsence[item.r_absence_type.NOM] = new List<string> { "ABSE_" + item.r_absence_type.ID + "_" + item.DATE_ABSENCE.Day + "_" + item.NOMBREHEURES };
                }

            }
            //completer la liste des avec les types d'absences non existante
            foreach (var item in _context.r_absence_type)
            {
                if (!ram.DetailsAbsence.ContainsKey(item.NOM))
                {
                    ram.DetailsAbsence[item.NOM] = new List<string> { "ABSE_" + item.ID + "_" + " " + "_" + 0 };
                }
            }
            //definition du mode de la page
            ViewData["mode"] = "update";
            ViewData["collaborateurID"] = new SelectList(_context.Collaborateur, "ID", "ID", ram.collaborateurID);
            return View(ram);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(string[] tableauRam, int id)
        {
            //recuperation des infos du ram
            var ram = await GetInfoRam(id);
            //mise a jours du ram avec les informations saisies par l'utilisateurs
            ram = await ModifierDetailsRam(tableauRam, ram);
            //rediriger vers la page details 
            return RedirectToAction(nameof(Details), ram);
        }
        private async Task<Ram> ModifierDetailsRam(string[] tableauRam, Ram ram)
        {
            double _NombreJoursTravaille = 0;
            double _NombreJourAbsence = 0;
            //var _anneeMois = ram.AnneeMois.Split('-');
            int _Annee = ram.ANNEE;
            int _Mois = ram.MOIS;
            ram.ListeRamDetailsAbsence = new Collection<RamDetailsAbsence>();
            ram.ListeRamDetailsPresence = new Collection<RamDetailsPresence>();
            //suppression du details de presence et absence existante pour ce ram
            _context.RamDetailsAbsence.RemoveRange(ram.ListeRamDetailsAbsence);
            _context.RamDetailsPresence.RemoveRange(ram.ListeRamDetailsPresence);
            //saubegarder le contexte
            await _context.SaveChangesAsync();
            //Parcour du tableau pour determiner le nombre de jours d'absence, et le nombre de jours travaillés 
            foreach (var item in tableauRam)
            {
                //recuperer les informations saisies dans le tableau
                var tab = item.Split('_');
                var _TypeMission = tab[0];
                var _TypeAffaire = tab[1];
                var _JourConcerne = tab[2];
                var _NombreHeureTravaille = tab[3];
                if (_NombreHeureTravaille != "0")
                {
                    // verifier quel type de saisie : presence ou absence 
                    switch (_TypeMission)
                    {
                        case "PRES": // presence
                            RamDetailsPresence _presence = new RamDetailsPresence();
                            var affairecollaborateuriD = _context.AffaireCollaborateur.Where(d => d.AFFAIREID == int.Parse(_TypeAffaire)).FirstOrDefault();

                            _presence.AffaireCollaborateurID = affairecollaborateuriD.ID;
                            _presence.RAMID = ram.ID;
                            _presence.DATE_TRAVAILLE = new DateTime(_Annee, _Mois, int.Parse(_JourConcerne));
                            //_presence.ram = ram;
                            _presence.NOMBREHEURE = double.Parse(_NombreHeureTravaille);
                            _context.Add(_presence);
                            _NombreJoursTravaille += double.Parse(_NombreHeureTravaille);
                            break;

                        case "ABSE": //Absence
                            RamDetailsAbsence _absence = new RamDetailsAbsence();
                            _absence.DATE_ABSENCE = new DateTime(_Annee, _Mois, int.Parse(_JourConcerne));
                            _absence.R_absence_typeID = int.Parse(_TypeAffaire);
                            _absence.RAMID = ram.ID;
                            _absence.NOMBREHEURES = double.Parse(_NombreHeureTravaille);
                            _context.Add(_absence);
                            _NombreJourAbsence += double.Parse(_NombreHeureTravaille);
                            break;
                    }
                    // mise a jours du context 
                    await _context.SaveChangesAsync();
                }
            }
            //mise a jours du nombre de jours travaillée et les jours d'absence dans l'objet RAM            
            _context.Ram.Where(d => d.ID == ram.ID).FirstOrDefault().JOURS_ABSENCE = _NombreJourAbsence;
            _context.Ram.Where(d => d.ID == ram.ID).FirstOrDefault().JOURS_TRAVAILLES = _NombreJoursTravaille;
            // mise à jours de la base de données
            await _context.SaveChangesAsync();
            return ram;
        }
        #endregion

        #region suppression
        // GET: Rams/Delete/5


        //// POST: Rams/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var ram = await _context.Ram
        //        .Include(r => r.ListeRamDetailsAbsence)
        //        .Include(d => d.ListeRamDetailsPresence)
        //        .FirstOrDefaultAsync(r => r.ID == id);

        //    _context.Ram.Remove(ram);
        //    await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //    return Content("Lister");
        //}

        // POST: Rams/Delete/5
        [HttpGet, ActionName("Delete")]
        //action de suppression aprés confirmation de l'utilisateur
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //recuperation du ram avec L'id reçu
            var ram = await _context.Ram
                .Include(r => r.ListeRamDetailsAbsence)
                .Include(d => d.ListeRamDetailsPresence)
                .FirstOrDefaultAsync(r => r.ID == id);
            //suppression du ram du contexte
            _context.Ram.Remove(ram);
            // mise a jours de la base de données 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Lister));
        }
        #endregion

        #region Generation Tableau RAM

        // fonction de génération du tableau en fonction du mois et de l'année saisie.
        public async Task<IActionResult> GenererTableauRam(int mois, int annee)
        {
            Ram ram = new Ram();
            ram.ANNEE = annee;
            ram = await _context.Ram
                    .Include(d => d.collaborateur)
                    .ThenInclude(d => d.ListeAffaireCollaborateur)
                        .ThenInclude(c => c.affaire)
                        .ThenInclude(c => c.AFFAIRETYPE)
                        .FirstOrDefaultAsync(d => d.collaborateur.ID == 2);
            if (ram == null)
            {
                return NotFound();
            }

            ram.Details = new Dictionary<string, string>();
            //recuperation de la liste des affaires du collaborateur
            foreach (var elt in ram.collaborateur.ListeAffaireCollaborateur)
            {
                ram.Details.Add(elt.affaire.NOM, "PRES_" + elt.AFFAIREID);
            }
            //recuperation de la liste des absences de la table de parametrage 
            foreach (var item in _context.r_absence_type)
            {
                ram.Details.Add(item.NOM, "ABSE_" + item.ID);
            }
            ram.MOIS = mois;
            return PartialView("_CreateDetailsPresence", ram);
        }
        #endregion

        #region canvas 

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromBody] ImageSend image)
        {
            // var test = image.Image.Count();
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Images\\");
            var ram = await _context.Ram.
                Include(c => c.collaborateur)
                .FirstOrDefaultAsync(d => d.ID == int.Parse(image.id));
            string _fileName = ram.collaborateur.NOM + ram.collaborateur.PRENOM + DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";
            string fileNameWitPath = uploads + _fileName.Replace(" ", "");
            fileNameWitPath = fileNameWitPath.Replace(" ", "");
            using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert
                        .FromBase64String(image.Image);
                    bw.Write(data);
                    bw.Close();
                }
                fs.Close();
            }

            ram.SIGNATURE = _fileName.Replace(" ", "");
            _context.Update(ram);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), ram);
        }
        #endregion
    }
    public class ImageSend
    {
        public ImageSend()
        { }
        public string Image { get; set; }
        public string id { get; set; }
    }
}
