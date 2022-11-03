import { Layout } from '@/Components/Shared/Layout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { HttpClient } from '@/Shared/HttpClient';
import { StatsStore } from '@/Stores/StatsStore';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

const httpClient = new HttpClient();

const statsStore = new StatsStore(httpClient);

const StatsIndex = observer(
	(): React.ReactElement => {
		const title = 'Statistics / Reports'; /* LOCALIZE */

		useVdbTitle(title, true);

		return (
			<Layout title={title}>
				<select
					value={JSON.stringify(statsStore.selectedReport)}
					onChange={(e): void =>
						runInAction(() => {
							statsStore.selectedReport = JSON.parse(e.target.value);
						})
					}
					className="input-large"
				>
					{statsStore.categories.map((category) => (
						<optgroup label={category.name} key={category.name}>
							{category.reports.map((report) => (
								<option value={JSON.stringify(report)} key={report.name}>
									{report.name}
								</option>
							))}
						</optgroup>
					))}
				</select>
				{statsStore.showTimespanFilter && (
					<>
						{' '}
						<select
							value={statsStore.timespan}
							onChange={(e): void =>
								runInAction(() => {
									statsStore.timespan = e.target.value;
								})
							}
						>
							<option value="">Overall{/* LOCALIZE */}</option>
							<option value="24">Last day{/* LOCALIZE */}</option>
							<option value="48">Last two days{/* LOCALIZE */}</option>
							<option value="168">Last week{/* LOCALIZE */}</option>
							<option value="720">Last month{/* LOCALIZE */}</option>
							<option value="8760">Last year{/* LOCALIZE */}</option>
							<option value="17520">Last 2 years{/* LOCALIZE */}</option>
							<option value="26280">Last 3 years{/* LOCALIZE */}</option>
							<option value="43800">Last 5 years{/* LOCALIZE */}</option>
						</select>
					</>
				)}
				{statsStore.chartData && (
					<HighchartsReact
						highcharts={Highcharts}
						options={statsStore.chartData}
						immutable={true}
					/>
				)}
			</Layout>
		);
	},
);

export default StatsIndex;
