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
using Microsoft.AspNetCore.Hosting;

namespace RamMyERP3.Controllers
{
    public class RamsController : Controller
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RamsController(MyContext context, IWebHostEnvironment hostingEnvironment)
        {
            this._context = context;
            _appEnvironment = hostingEnvironment;
        }

        #region Lister
        // GET: Rams
        public async Task<IActionResult> Lister()
        {
            // Récupérer les données de la liste
            var liste = _context.Ram
                .Include(d => d.collaborateur).OrderBy(d=>d.MOIS);

            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";

            // Afficher la vue
            return View(await liste.ToListAsync());
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
                    ram.DetailsAbsence.Add(item.affaireCollaborateur.affaire.NOM, new List<string> { item.DATE_TRAVAILLE.Day + "_" + item.nombreHeure });

                else
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM].Add(item.DATE_TRAVAILLE.Day + "_" + item.nombreHeure);

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
                    .ThenInclude(listeRamDetailsAbsence => listeRamDetailsAbsence.r_absence_type)

                .FirstOrDefaultAsync(m => m.ID == id);
        }
        #endregion


        #region Ajout

        //  Action d'affichage de la vue saisie RAM lors du clique sur Ajouter (Http Get)
        public IActionResult CreationRam()
        {
            Ram ram = new Ram
            {
                collaborateur = _context.Collaborateur.Where(e => e.ID == 2).Include(d => d.ListeAffaireCollaborateur)
                    .ThenInclude(c => c.affaire)
                    .ThenInclude(c => c.AFFAIRETYPE)
                    .FirstOrDefault(),
                collaborateurID = 2
            };

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
            if (ModelState.IsValid)       // model valide
            {
                // Mettre à jours du Ram et insertion dans la base de données 
                ram = await InsererRam(tableauRam, ram);
                return RedirectToAction(nameof(Details), ram);
            }
            // model non valide, affichage des erreur sur la page 
            return CreationRam();

        }
        //Methode d'insertion du ram dans la base de données 
        private async Task<Ram> InsererRam(string[] tableauRam, Ram ram)
        {
            double nombreJoursTravaille = 0;
            double nombreJourAbsence = 0;
            var anneeMois = ram.ANNEEMOIS.Split('-');
            int annee = int.Parse(anneeMois[0]);
            int mois = int.Parse(anneeMois[1]);
            ram.MOIS = mois;
            ram.ANNEE = annee;
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
                var typeMission = tab[0];
                var typeAffaire = tab[1];
                var jourConcerne = tab[2];
                var nombreHeureTravaille = tab[3];
                ram.MOIS = mois;
                ram.ANNEE = annee;
                if (nombreHeureTravaille != "0")
                {
                    // verifier quel type de saisie : presence ou absence 
                    switch (typeMission)
                    {
                        case "PRES": // presence
                            RamDetailsPresence presence = new RamDetailsPresence();
                            var affairecollaborateuriD = _context.AffaireCollaborateur.FirstOrDefault(d => d.AFFAIREID == int.Parse(typeAffaire));

                            if (affairecollaborateuriD != null)
                                presence.AffaireCollaborateurID = affairecollaborateuriD.ID;
                            presence.RAMID = ram.ID;
                            presence.DATE_TRAVAILLE = new DateTime(annee, mois, int.Parse(jourConcerne));
                            presence.nombreHeure = double.Parse(nombreHeureTravaille);
                            nombreJoursTravaille += double.Parse(nombreHeureTravaille);
                            ram.ListeRamDetailsPresence.Add(presence);
                            break;

                        case "ABSE": //Absence
                            RamDetailsAbsence absence = new RamDetailsAbsence();
                            absence.DATE_ABSENCE = new DateTime(annee, mois, int.Parse(jourConcerne));
                            absence.R_absence_typeID = int.Parse(typeAffaire);
                            absence.RAMID = ram.ID;
                            absence.NOMBREHEURES = double.Parse(nombreHeureTravaille);
                            ram.ListeRamDetailsAbsence.Add(absence);
                            nombreJourAbsence += double.Parse(nombreHeureTravaille);

                            break;
                    }
                }
            }
            // Mettre à jours du nombre de jours travaillée et les jours d'absence dans l'objet RAM
            ram.JOURS_ABSENCE = nombreJourAbsence;
            ram.JOURS_TRAVAILLES = nombreJoursTravaille;
            ram.collaborateur = null;
            // Mettre à jours le context 
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
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM].Add("PRES_" + item.affaireCollaborateur.AFFAIREID + "_" + item.DATE_TRAVAILLE.Day + "_" + item.nombreHeure);
                }
                catch
                {
                    ram.DetailsAbsence[item.affaireCollaborateur.affaire.NOM] = new List<string> { "PRES_" + item.affaireCollaborateur.AFFAIREID + "_" + item.DATE_TRAVAILLE.Day + "_" + item.nombreHeure };
                }
            }
            // Récuperation de la liste du des affaires du collaborateur 
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
            // ViewData["collaborateurID"] = new SelectList(context.Collaborateur, "ID", "ID", ram.collaborateurID);
            return View(ram);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(string[] tableauRam, int id)
        {
            //recuperation des infos du ram
            var ram = await GetInfoRam(id);
            // Mettre à jours du ram avec les informations saisies par l'utilisateurs
            ram = await ModifierDetailsRam(tableauRam, ram);
            // Rediriger vers la page détails 
            return RedirectToAction(nameof(Details), ram);
        }
        private async Task<Ram> ModifierDetailsRam(string[] tableauRam, Ram ram)
        {
            double nombreJoursTravaille = 0;
            double nombreJourAbsence = 0;
            int annee = ram.ANNEE;
            int mois = ram.MOIS;
            ram.ListeRamDetailsAbsence = new Collection<RamDetailsAbsence>();
            ram.ListeRamDetailsPresence = new Collection<RamDetailsPresence>();
            //suppression du details de presence et absence existante pour ce ram
            _context.RamDetailsAbsence.RemoveRange(ram.ListeRamDetailsAbsence);
            _context.RamDetailsPresence.RemoveRange(ram.ListeRamDetailsPresence);
            // Sauvegarder le contexte
            await _context.SaveChangesAsync();
            //Parcour du tableau pour determiner le nombre de jours d'absence, et le nombre de jours travaillés 
            foreach (var item in tableauRam)
            {
                //recuperer les informations saisies dans le tableau
                var tab = item.Split('_');
                var typeMission = tab[0];
                var typeAffaire = tab[1];
                var jourConcerne = tab[2];
                var nombreHeureTravaille = tab[3];
                if (nombreHeureTravaille != "0")
                {
                    // verifier quel type de saisie : presence ou absence 
                    switch (typeMission)
                    {
                        case "PRES": // presence
                            RamDetailsPresence presence = new RamDetailsPresence();
                            var affairecollaborateuriD = _context.AffaireCollaborateur.FirstOrDefault(d => d.AFFAIREID == int.Parse(typeAffaire));

                            if (affairecollaborateuriD != null)
                                presence.AffaireCollaborateurID = affairecollaborateuriD.ID;
                            presence.RAMID = ram.ID;
                            presence.DATE_TRAVAILLE = new DateTime(annee, mois, int.Parse(jourConcerne));
                            presence.nombreHeure = double.Parse(nombreHeureTravaille);
                            _context.Add(presence);
                            nombreJoursTravaille += double.Parse(nombreHeureTravaille);
                            break;

                        case "ABSE": //Absence
                            RamDetailsAbsence absence = new RamDetailsAbsence();
                            absence.DATE_ABSENCE = new DateTime(annee, mois, int.Parse(jourConcerne));
                            absence.R_absence_typeID = int.Parse(typeAffaire);
                            absence.RAMID = ram.ID;
                            absence.NOMBREHEURES = double.Parse(nombreHeureTravaille);
                            _context.Add(absence);
                            nombreJourAbsence += double.Parse(nombreHeureTravaille);
                            break;
                    }
                    // mise a jours du context 
                    await _context.SaveChangesAsync();
                }
            }

            // Mettre à jours le nombre de jours travaillée et les jours d'absence dans l'objet RAM            
            ram.JOURS_ABSENCE = nombreJourAbsence;
            ram.JOURS_TRAVAILLES = nombreJoursTravaille;
            _context.Update(ram);
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
        //    var ram = await context.Ram
        //        .Include(r => r.ListeRamDetailsAbsence)
        //        .Include(d => d.ListeRamDetailsPresence)
        //        .FirstOrDefaultAsync(r => r.ID == id);

        //    context.Ram.Remove(ram);
        //    await context.SaveChangesAsync();
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
            var ram = await _context.Ram
                .Include(d => d.collaborateur)
                .ThenInclude(d => d.ListeAffaireCollaborateur)
                .ThenInclude(c => c.affaire)
                .ThenInclude(c => c.AFFAIRETYPE)
                .FirstOrDefaultAsync(d => d.collaborateur.ID == 2);
            ram.ANNEE = annee;
            ram.MOIS = mois;
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
            string fileName = ram.collaborateur.NOM + ram.collaborateur.PRENOM + DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";
            string fileNameWitPath = uploads + fileName.Replace(" ", "");
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

            ram.SIGNATURE = fileName.Replace(" ", "");
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

