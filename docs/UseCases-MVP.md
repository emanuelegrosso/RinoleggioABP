# Use case MVP — Rinoleggio

Documento funzionale del progetto ABP `Rinoleggio` (layered monolith, MVC, EF Core). Fonte requisiti: `Pagine.md` nella root del repository. Il backoffice coordina il subentro fuori piattaforma; non sono previsti ordini o pagamenti online.

## Attori

| Attore | Ruolo applicativo | Scopo |
|---|---|---|
| Visitatore | Anonimo | Cerca e consulta annunci pubblici. |
| Utente | Utente autenticato | Agisce come acquirente e/o venditore. |
| Backoffice | Ruolo `BackOffice` | Gestisce annunci e richieste, consulta i recapiti e coordina le parti. |
| Amministratore | `Administrator` | Include il backoffice, gestisce anagrafiche e ripristina annunci cancellati. |

## Use case

| ID | Nome | Attore principale | Risultato |
|---|---|---|---|
| UC-01 | Consultare il servizio | Tutti | Dalla home raggiunge ricerca e pagine informative. |
| UC-02 | Cercare contratti | Tutti | Trova annunci pubblicati, attivi o riservati e non scaduti tramite filtri, ordinamento e paginazione. |
| UC-03 | Consultare annuncio | Tutti | Vede dettagli e immagini senza recapiti privati. |
| UC-04 | Registrarsi, accedere o recuperare password | Visitatore | Accede alle funzioni riservate; dopo l’autenticazione torna all’annuncio richiesto. |
| UC-05 | Inviare richiesta | Utente | Invia messaggio e consenso privacy per un annuncio attivo; la richiesta viene registrata come `New` e notificata al backoffice. |
| UC-06 | Vedere le richieste inviate | Acquirente | Consulta annuncio, data, messaggio e stato delle proprie richieste. |
| UC-07 | Inserire/modificare annuncio | Utente / proprietario | Crea una bozza o pubblica; modifica i propri dati di annuncio, contratto, veicolo, servizi e immagini. |
| UC-08 | Gestire i propri annunci | Venditore | Consulta stati e richieste ricevute; modifica, ritira, ripubblica un proprio ritiro, riserva, chiude o cancella logicamente un annuncio. |
| UC-09 | Gestire profilo | Utente | Aggiorna nome visibile, telefono privato, tipo account e consenso privacy. |
| UC-10 | Consultare e moderare annunci | Backoffice | Cerca ogni stato e apre/modifica, sospende/riattiva, riserva, chiude, ritira o cancella annunci. Inserisce il motivo per sospensione, ritiro e cancellazione. |
| UC-11 | Gestire richieste ricevute | Backoffice | Consulta dati di acquirente e venditore, messaggio e note interne; aggiorna stato e invia notifica email all’acquirente. |
| UC-12 | Coordinare il subentro | Backoffice | Contatta le parti e la società di noleggio fuori piattaforma; registra esito tramite stato richiesta e annuncio. |
| UC-13 | Gestire anagrafiche e ripristinare | Amministratore | Gestisce società di noleggio, marche e province; consulta e ripristina annunci cancellati. |

## Flussi principali

1. **Acquirente** — home/ricerca → dettaglio → login o registrazione → richiesta → conferma → richieste personali e aggiornamenti email.
2. **Venditore** — registrazione → inserimento in bozza → pubblicazione → gestione dell’annuncio → consultazione richieste ricevute.
3. **Backoffice** — nuova richiesta → contatto fuori piattaforma → stato `Contacted` e note interne → chiusura richiesta e aggiornamento annuncio.
4. **Moderazione** — sospensione con motivo → correzione del venditore → riattivazione backoffice.

## Regole e autorizzazioni

- Solo utenti autenticati possono creare richieste o annunci; un utente non può richiedere il proprio annuncio.
- È consentita una sola richiesta aperta (`New` o `Contacted`) per acquirente e annuncio; la richiesta richiede annuncio `Active`.
- Il venditore vede richieste sui propri annunci, ma non telefono/email dell’acquirente né note interne. I recapiti sono riservati al backoffice.
- Il proprietario può modificare l’annuncio finché non è `Closed` o `Deleted`. Non può riattivare un annuncio `Suspended`.
- Il venditore può ripubblicare un annuncio ritirato da sé se il contratto non è scaduto; un ritiro backoffice richiede riattivazione backoffice.
- Sospensione, ritiro e cancellazione richiedono un motivo. `Deleted` è cancellazione logica: non compare agli utenti né al backoffice; solo l’amministratore può ripristinare.
- Ogni cambio stato richiesta verso `Contacted` o `Closed` invia email all’acquirente; le note interne non sono incluse.
- L’accesso a ogni operazione va verificato lato server secondo ruolo e proprietà del record, oltre a nascondere le azioni non consentite in UI.

## Fuori perimetro MVP

Ordini, checkout e pagamenti; trasferimento automatico del contratto; chat o scambio diretto di recapiti tra utenti; login social.
