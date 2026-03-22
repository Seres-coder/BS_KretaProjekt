const API_BASE = "https://localhost:7273/api/Data";

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGomb();
    panelValtas();

    panelMutatas("bejovo-uzenetek-panel");

    if (typeof diakAdatokBetoltese === "function") {
        await diakAdatokBetoltese();
    }
});

//#region sidebar működése és a panel váltás

function sidebarGomb() {
    const sidebar = document.getElementById("sidebar");
    const gomb = document.getElementById("sidebar-toggle");

    if (sidebar && gomb) {
        gomb.addEventListener("click", function () {
            sidebar.classList.toggle("closed");
        });
    }
}

function panelValtas() {
    const gombok = document.querySelectorAll("[data-panel-target]");

    gombok.forEach(gomb => {
        gomb.addEventListener("click", function () {
            const panelId = this.getAttribute("data-panel-target");
            panelMutatas(panelId);

            gombok.forEach(x => x.classList.remove("active"));
            this.classList.add("active");
        });
    });

    const elsoGomb = document.querySelector('[data-panel-target="bejovo-uzenetek-panel"]');
    if (elsoGomb) {
        elsoGomb.classList.add("active");
    }
}

function panelMutatas(panelId) {
    const panelek = document.querySelectorAll(".panel");

    panelek.forEach(panel => {
        panel.style.display = "none";
        panel.classList.remove("active");
    });

    const aktivPanel = document.getElementById(panelId);
    if (aktivPanel) {
        aktivPanel.style.display = "block";
        aktivPanel.classList.add("active");
    }
}

//#endregion

