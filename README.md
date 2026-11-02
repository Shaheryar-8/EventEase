# EventEase

**Live demo:** https://shaheryar-8.github.io/EventEase/

EventEase is a Blazor WebAssembly app for browsing corporate and social events, registering for them and tracking who attended. It was built step by step as the course project for *Using Microsoft Copilot to build Blazor applications*.

## Features

| Area | What it does |
| --- | --- |
| **Event Card component** | `Components/EventCard.razor` shows an event's name, date, location, description and seat usage. It supports two-way binding (`@bind-Event`): edits happen on a validated copy and are written back through `EventChanged`. |
| **Routing** | Pages for `/`, `/events`, `/events/{id:int}`, `/events/{id:int}/register`, `/attendance/{eventId:int?}`, `/my-registrations` and `/account`. Route constraints reject non-numeric ids, unknown ids show an "Event not found" message, and any other address shows a friendly "Page not found" page. |
| **Registration form** | `EditForm` with `DataAnnotationsValidator`: required name, valid email, optional phone, 1–5 seats, terms checkbox. The service also checks capacity, duplicate emails and past events, and the button is guarded against double clicks. |
| **User session** | `UserSessionService` remembers the signed-in visitor in `sessionStorage`, pre-fills the registration form and powers the "My registrations" page. |
| **Attendance tracker** | Pick an event, tick people off as they arrive, and see registered / attended / attendance rate / seats left update live. |
| **Persistence** | Events, registrations and attendance are kept in `localStorage`, so the data survives a page reload. |
| **Performance** | Debounced search (250 ms), `Virtualize` for the event list, `@key` on repeated items, data loaded once per tab and cached in the services, and `OnParametersSetAsync` so moving between `/events/1` and `/events/2` reloads correctly. |

## Run it

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run
```

Then open the URL shown in the terminal (for example `http://localhost:5000`).

## Project structure

```
Components/   EventCard, NotFoundMessage, SessionBadge
Layout/       MainLayout, NavMenu
Models/       Event, Registration, UserSession (with validation attributes)
Pages/        Home, Events, EventDetails, Register, Attendance, MyRegistrations, Account
Services/     EventService, RegistrationService, UserSessionService, BrowserStorage
```

## Development steps

1. **Foundation:** generated the `Event` model and the `EventCard` component with its fields and two-way data binding.
2. **Routing and debugging:** added the event list, details and registration routes; fixed invalid ids with `:int` route constraints, a "not found" state for unknown ids and a custom router `NotFound` page; fixed stale data when switching between event ids.
3. **Optimization:** added input validation on every form, debounced the search box, virtualized the event list and cached data in scoped services.
4. **Advanced features:** built the registration form with validation, session state for the signed-in user and the attendance tracker with live statistics.
