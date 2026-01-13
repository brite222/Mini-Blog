document.addEventListener("DOMContentLoaded", () => {
    const searchBox = document.getElementById("searchBox");
    const suggestions = document.getElementById("suggestions");

    if (!searchBox) return;

    let timeout = null;

    searchBox.addEventListener("input", () => {
        clearTimeout(timeout);

        const term = searchBox.value.trim();
        suggestions.innerHTML = "";

        if (term.length < 2) return;

        timeout = setTimeout(async () => {
            const response = await fetch(`/Home/SearchSuggestions?term=${encodeURIComponent(term)}`);
            const data = await response.json();

            if (!data.length) return;

            data.forEach(item => {
                const link = document.createElement("a");
                link.className = "list-group-item list-group-item-action";
                link.textContent = item.title;
                link.href = `/BlogPosts/Details/${item.id}`;
                suggestions.appendChild(link);
            });
        }, 300);
    });

    document.addEventListener("click", (e) => {
        if (!suggestions.contains(e.target) && e.target !== searchBox) {
            suggestions.innerHTML = "";
        }
    });
});
