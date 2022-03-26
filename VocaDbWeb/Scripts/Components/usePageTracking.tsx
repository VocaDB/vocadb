// Captures pageviews as a new page is loaded.
// The call to usePageTracking must go after useTitle.
// Set ready to false while translations are not yet loaded.
// This prevents Google Analytics from using an incomplete page title (e.g. `Index.Discussions - Vocaloid Database`).
const usePageTracking = (ready: boolean): void => {
	// Do nothing.
};

export default usePageTracking;
