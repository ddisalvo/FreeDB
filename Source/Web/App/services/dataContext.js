define(
    function () {
        var dataContext = {
            getDiscs: getDiscs,
            getDisc: getDisc,
            searchDiscs: searchDiscs,
            getArtists: getArtists,
            getArtist: getArtist
        };

        return dataContext;

        function paginationToOdata(paginationParameters) {
            var data = {
                $inlinecount: "allpages",
                $orderby: paginationParameters().sortCommand() + ' ' + (paginationParameters().sortDescending() ? 'desc' : 'asc'),
                $top: paginationParameters().pageSize(),
                $skip: (paginationParameters().currentPage() - 1) * paginationParameters().pageSize()
            };

            return data;
        }

        function searchDiscs(term, paginationParameters) {
            return $.ajax({
                url: '/api/discs/search?search=' + escape(term),
                dataType: 'json',
                data: paginationToOdata(paginationParameters)
            });
        }

        function getDiscs(paginationParameters) {
           return $.ajax({
               url: '/api/discs',
               dataType: 'json',
               data: paginationToOdata(paginationParameters)
           });
        }
        
        function getDisc(id) {
            return $.ajax({
                url: '/api/discs/' + id,
                dataType: 'json'
            });
        }
        
        function getArtists(paginationParameters) {
            return $.ajax({
                url: '/api/artists',
                dataType: 'json',
                data: paginationToOdata(paginationParameters)
            });
        }

        function getArtist(id) {
            return $.ajax({
                url: '/api/artists/' + id,
                dataType: 'json'
            });
        }
    });