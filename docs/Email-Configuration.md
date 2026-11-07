# Configurazione email

Rinoleggio usa il `IEmailSender` di ABP per notificare il Backoffice alla ricezione di una richiesta e l’acquirente quando cambia lo stato della richiesta.

## Configurare il server SMTP

Il modulo Setting Management di ABP espone le impostazioni email nella pagina amministrativa delle impostazioni. Configurare almeno:

- `Abp.Mailing.Smtp.Host`
- `Abp.Mailing.Smtp.Port`
- `Abp.Mailing.Smtp.UserName`
- `Abp.Mailing.Smtp.Password`
- `Abp.Mailing.Smtp.EnableSsl`
- `Abp.Mailing.Smtp.UseDefaultCredentials`
- `Abp.Mailing.DefaultFromAddress`
- `Abp.Mailing.DefaultFromDisplayName`

Il nome mittente predefinito è `Rinoleggio`; indirizzo mittente e credenziali dipendono dal provider scelto. Inserire la password nella pagina di gestione impostazioni, che la conserva tramite il sistema di setting ABP. Non committare credenziali in `appsettings.json`.

Per la configurazione tramite `appsettings.json`, ABP usa una sezione `Settings` con i nomi delle impostazioni come chiavi. La password deve essere cifrata usando `ISettingEncryptionService` prima di inserirla nella configurazione.

## Verifica

Inviare una richiesta di prova con un account Customer e verificare la notifica alla casella del Backoffice. Cambiare lo stato della richiesta e verificare l’email all’acquirente. Senza SMTP configurato, l’errore viene scritto nel log applicativo e la richiesta o l’aggiornamento restano salvati.

La consegna SMTP non può essere verificata fino a quando non sono configurati host, mittente e credenziali del provider.
